using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Models.Booking;
using RoomBooking.DataAccess.DbContext;
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
        catch (Exception ex)
        {
            logger.LogWarning(
                "An error occured while creating a booking {@BookingId} for room {@RoomId} at {@StartTime}-{@EndTime}",
                booking.Id, booking.RoomId, booking.StartTime, booking.EndTime);
            
            throw new BookingCreationException(booking.Id);
        }
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await dbContext.Bookings
            .Where(b => b.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted > 0;
    }
}