using Microsoft.Extensions.Logging;
using RoomBooking.Application.Validations.Abstractions.Bookings;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Services;

public class BookingService(
    IBookingRepository repository,
    IBookingCreationValidator validator,
    ILogger<BookingService> logger) : IBookingService
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
        logger.LogInformation("{@MethodName}: Creating booking: {@Booking}", nameof(Create), booking);
        var errors = await validator.Validate(booking, cancellationToken);
        
        if (errors.Any())
        {
            logger.LogInformation("{@MethodName}: Validation errors occured while creating booking: {@Errors}",
                nameof(Create), errors);
            return Result<Guid>.Failures(errors);
        }
        
        var id = await repository.Create(booking, cancellationToken);
        logger.LogInformation("{@MethodName}: Booking created successfully: {@Id}",
            nameof(Create), id);
        return id;
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