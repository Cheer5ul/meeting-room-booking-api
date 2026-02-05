using RoomBooking.Application.Validations.Abstractions;

namespace RoomBooking.Application.Validations.Exceptions;

public class BookingValidateException : BadRequestException
{
    public BookingValidateException(List<string> errors)
        : base("Booking validation failed", errors)
    {
        
    }
}