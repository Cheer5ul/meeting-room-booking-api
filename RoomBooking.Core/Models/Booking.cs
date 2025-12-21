using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Core.Models;

public class Booking
{
    public Booking(Guid bookingId, Guid roomId, Guid userId, DateTime startDate, DateTime endDate, string? purpose = "")
    {
        Id = bookingId;
        RoomId = roomId;
        UserId = userId;
        StartTime = startDate;
        EndTime = endDate;
        Purpose = purpose ?? string.Empty;
    }
    public Guid Id { get; }
    public Guid RoomId { get; }
    public Guid UserId { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    public string? Purpose { get; } 

    public static (Booking booking, List<Error>? errors) 
        Create(Guid bookingId, Guid roomId, Guid userId, DateTime startDate, DateTime endDate, string? purpose = "")
    {
        List<Error>? errors = null;
        // if (userId == Guid.Empty || roomId == Guid.Empty || userId == roomId)
        //     errors?.Add(BookingErrors.InvalidIDs);
        // if(startDate == DateTime.MinValue || endDate == DateTime.MinValue || startDate < endDate || startDate - endDate > MaxDiff
        //    || startDate == endDate)
        //     error = "Date time is invalid";
        
        var booking = new Booking(bookingId, roomId, userId, startDate, endDate, purpose);

        return (booking, errors);
    }
    
}