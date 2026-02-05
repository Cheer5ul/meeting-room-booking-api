namespace RoomBooking.DataAccess.Exceptions;

public class BookingOverlapException : Exception
{
    public Guid RoomId { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }

    public BookingOverlapException(Guid roomId, DateTime startTime, DateTime endTime)
        : base($"Room {roomId} is already booked for {startTime} - {endTime}")
    {
        RoomId = roomId;
        StartTime = startTime;
        EndTime = endTime;
    }
}