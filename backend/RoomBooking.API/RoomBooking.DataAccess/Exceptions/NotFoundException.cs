namespace RoomBooking.DataAccess.Exceptions;

public class NotFoundException : BadRequestException
{
    public NotFoundException(string error)
        : base(error)
    {
        
    }
}
