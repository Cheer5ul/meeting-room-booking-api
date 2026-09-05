namespace RoomBooking.DataAccess.Exceptions;

public class EmailAlreadyInUseException : Exception
{
    public EmailAlreadyInUseException(string email, Exception innerException)
        : base($"Email {email}  is already in use", innerException) { }
}