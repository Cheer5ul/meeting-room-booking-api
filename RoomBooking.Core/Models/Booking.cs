namespace RoomBooking.Core;

public class Booking
{
    public static readonly TimeSpan MAX_DIFF = new TimeSpan(6, 0, 0);
    public Booking(Guid bookingId, Guid roomId, Guid userId, DateTime startDate, DateTime endDate, string purpose = "")
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

    public static (Booking booking, string? error)
        Create(Guid bookingId, Guid roomId, Guid userId, DateTime startDate, DateTime endDate, string purpose = "")
    {
        string error = string.Empty;
        
        if (userId == Guid.Empty || roomId == Guid.Empty)
            error = "IDs cannot be null";
        if(startDate == DateTime.MinValue || endDate == DateTime.MinValue || startDate < endDate || startDate - endDate > MAX_DIFF
           || startDate == endDate)
            error = "Date time is invalid";
        
        var booking = new Booking(bookingId, roomId, userId, startDate, endDate, purpose);
        
        return (booking, error);
    }
}