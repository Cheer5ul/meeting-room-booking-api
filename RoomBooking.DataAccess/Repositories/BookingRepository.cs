using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.Booking;
using RoomBooking.DataAccess.Constraints;
using RoomBooking.DataAccess.DbContext;
using RoomBooking.DataAccess.Entities;
using RoomBooking.DataAccess.Entities.BookingEntity;
using RoomBooking.DataAccess.Exceptions;

namespace RoomBooking.DataAccess.Repositories;

public class BookingRepository(
    RoomBookingDbContext dbContext,
    ILogger<BookingRepository> logger) : IBookingRepository
{
    public async Task<List<Booking>> Get(CancellationToken cancellationToken = default)
    {
        var bookingEntities = await dbContext.Bookings
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        var bookings = bookingEntities
            .Select(b => Booking.Create(
                b.Id, 
                b.RoomId,
                b.UserId, 
                b.StartTime, 
                b.EndTime,
                b.Purpose ?? string.Empty).booking)
            .ToList();
        
        return bookings;
    }

    public async Task<List<Booking>> GetByRoom(Guid id, CancellationToken cancellationToken = default)
    {
        var bookingEntities = await dbContext.Bookings
            .AsNoTracking()
            .Where(b => b.RoomId == id)
            .ToListAsync(cancellationToken);
        
        var bookings = bookingEntities
            .Select(b => Booking.Create(
                b.Id, 
                b.RoomId,
                b.UserId, 
                b.StartTime, 
                b.EndTime,
                b.Purpose ?? string.Empty).booking)
            .ToList();
        
        return bookings;
    }

    public async Task<List<Booking>> GetByUser(Guid id, CancellationToken cancellationToken = default)
    {
        var bookingEntities = await dbContext.Bookings
            .AsNoTracking()
            .Where(b => b.UserId == id)
            .ToListAsync(cancellationToken);
        
        var bookings = bookingEntities
            .Select(b => Booking.Create(
                b.Id, 
                b.RoomId,
                b.UserId, 
                b.StartTime, 
                b.EndTime,
                b.Purpose ?? string.Empty).booking)
            .ToList();

        return bookings;
    }

    public async Task<Guid> Create(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            var bookingEntitiy = new BookingEntity
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                UserId = booking.UserId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Purpose = booking.Purpose,
            };
        
        
            await dbContext.Bookings.AddAsync(bookingEntitiy, cancellationToken);
            await  dbContext.SaveChangesAsync(cancellationToken);

            return bookingEntitiy.Id;
        }
        catch (DbUpdateException ex) when (IsOverlapConstraintViolation(ex))
        {
            logger.LogWarning("Booking overlap detected for room {@RoomId} at {@StartTime}-{@EndTime}",
                booking.RoomId, 
                booking.StartTime,
                booking.EndTime
                );
            
            throw new BookingOverlapException(
                booking.RoomId,
                booking.StartTime,
                booking.EndTime);
        }
    }

    private static bool IsOverlapConstraintViolation(DbUpdateException exception)
    {
        if (exception.InnerException is PostgresException postgresException)
        {
            return postgresException.SqlState == CustomPostgresErrorCodes.ExclusionConstraintViolation &&
                   postgresException.ConstraintName == DatabaseConstraints.BookingOverlapConstraint;
        }
        
        return false;
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await dbContext.Bookings
            .Where(b => b.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted > 0;
    }
}