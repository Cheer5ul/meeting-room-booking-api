namespace RoomBooking.Application.Validations.Exceptions;

public abstract class BadRequestException : Exception
{
    protected BadRequestException(string message, List<string>? errors = null) 
        :base(message)
    {
        Errors = errors ?? new List<string>(); 
    }
    public List<string> Errors { get; }
}