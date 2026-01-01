using System.Globalization;
using System.Numerics;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using RoomBooking.Application.Validations.Abstractions.Validators;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models.Booking;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;
using RoomBooking.DataAccess.DbContext;

namespace RoomBooking.Application.Services;

public class BookingService(
    IBookingRepository repository,
    IValidator<Booking> validator,
    IValidationToErrorConverter  toErrorConverter,
    ILogger<BookingService> logger,
    RoomBookingDbContext context) : IBookingService
{
    public async Task<Result<List<Booking>>> GetAllBookings(CancellationToken cancellationToken = default)
    {
        return await repository.Get(cancellationToken);
    }

    public async Task<Result<List<Booking>>> GetByRoom(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Getting bookings by roomId {@Id}", 
            nameof(GetByRoom), id);
        
        return await repository.GetByRoom(id, cancellationToken);
    }

    public async Task<Result<List<Booking>>> GetByUser(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Getting bookings by userId {@UserId}",
            nameof(GetByUser), id);
        
        return await repository.GetByUser(id, cancellationToken);
    }

    public async Task<Result<Guid>> Create(Booking booking, CancellationToken cancellationToken = default)
    {
        //Validation before the transaction
        var validationResult = await validator.ValidateAsync(booking, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("{@MethodName}: Validation errors occured while creating booking: {@Errors}",
                nameof(Create), validationResult.Errors);

            var errors = toErrorConverter.ValidationToErrors(validationResult.Errors);

            return Result<Guid>.Failures(errors);
        }
        
        //Generating lock id
        long lockId = GenerateBookingLockId(booking.RoomId, booking.StartTime);
        
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            bool lockAcquired = await context.Database
                .SqlQueryRaw<bool>("SELECT pg_try_advisory_xact_lock({0}) AS \"Value\" ", lockId)
                .FirstAsync(cancellationToken);

            if (!lockAcquired)
            {
                return Result<Guid>.Failures([BookingErrors.TryAgainLater]);
            }

            logger.LogInformation("{@MethodName}: Acquired lock for Room {@BookingId}",
                nameof(Create), booking.Id);

            if (await HasOverlappingAsync(booking, cancellationToken))
            {
                return Result<Guid>.Failures([BookingErrors.IsOverlapping]);
            }
            
            //Creation
            var id = await repository.Create(booking, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("{@MethodName}: Booking created successfully: {@Id}",
                nameof(Create), id);
            return id;
        }
        catch (Exception exception)
        {
            logger.LogWarning("{@MethodName}: An error occurred while creating a booking: {BookingId}, {@Exception}", 
                nameof(Create), booking.Id, exception);
            // await transaction.RollbackAsync(cancellationToken); //implicit lock releasing, changes rolled back
            throw;
        }
    }

    private long GenerateBookingLockId(Guid roomId, DateTime bookingDate)
    {
        var dateLock = bookingDate.Year * 10_000 + bookingDate.Month * 10 + bookingDate.Day;
        string idLock = roomId.ToString().Substring(0, 7);

        string hex = dateLock.ToString() + idLock;
        
        BigInteger lockId = BigInteger.Parse(hex, NumberStyles.AllowHexSpecifier); 
        
        return (long)lockId;
    }

    private async Task<bool> HasOverlappingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        var existingItems = await repository.GetByRoom(booking.RoomId, cancellationToken);
        
        var bookingsToCheck = existingItems
            .Where(b => b.Id != booking.Id) //excluding the current one while updating | just in case
            .ToList();

        //Need to make better approach, not to check ALL THE existing bookings
        return bookingsToCheck.Any(b =>
            booking.StartTime < b.EndTime &&
            b.StartTime < booking.EndTime);

    }
    
    public async Task<Result<Guid>> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var wasDeleted = await repository.Delete(id, cancellationToken);
        
        if (!wasDeleted)
        {
            logger.LogInformation("{@MethodName}: Deletion did not succeed, Booking id: {@Id}"
                , nameof(Delete), id);
            return Result<Guid>.Failures([BookingErrors.NotFound]);
        }
        
        logger.LogInformation("{@MethodName}: Booking with id {@Id} deleted successfully", 
            nameof(Delete), id);
        
        return id;
    }
}