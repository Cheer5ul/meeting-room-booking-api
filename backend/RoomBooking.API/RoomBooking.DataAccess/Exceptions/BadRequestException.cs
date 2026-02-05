namespace RoomBooking.DataAccess.Exceptions;

public class BadRequestException : Exception
{
    protected BadRequestException(string message) : base(message) { }
}