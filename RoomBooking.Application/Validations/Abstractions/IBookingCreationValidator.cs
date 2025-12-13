using RoomBooking.Core;

namespace RoomBooking.Application.Validations.Abstractions;

public interface IBookingCreationValidator
{
    Task<List<string>> Validate(
        Booking booking,
        CancellationToken cancellationToken = default);
}