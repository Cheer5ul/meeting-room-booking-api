namespace RoomBooking.DataAccess.Exceptions;

public class BookingCreationException : Exception
{
    public Guid BookingId { get; }
    public BookingCreationException(Guid  bookingId)
        : base($"An exception occurred while trying to create a new room.")
    {
        BookingId = bookingId;
    }
}