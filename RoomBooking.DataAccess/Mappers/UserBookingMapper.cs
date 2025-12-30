using RoomBooking.Core;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.Booking;
using RoomBooking.DataAccess.Entities;
using RoomBooking.DataAccess.Entities.BookingEntity;

namespace RoomBooking.DataAccess.Mappers;

public class UserBookingMapper
{
    //To Domain
    public static Booking ToDomain(BookingEntity entity)
    {
        return Booking.Create
        (
            entity.Id,
            entity.RoomId,
            entity.UserId,
            entity.StartTime,
            entity.EndTime,
            entity.Purpose).booking;

    }
    public static List<Booking> ToDomainList(List<BookingEntity>? bookingEntities)
    {
        return bookingEntities?.Select(entity => ToDomain(entity)).ToList() ?? new List<Booking>();
    }

    
    //To Entity
    public static BookingEntity ToEntity(Booking booking)
    {
        return new BookingEntity
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            UserId = booking.UserId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Purpose = booking.Purpose //nullable
        };

    }
    
    public static List<BookingEntity> ToEntityList(IEnumerable<Booking>? bookings)
    {
        return bookings?.Select(new Func<Booking,BookingEntity>(ToEntity)).ToList() ?? new List<BookingEntity>();
    }
}