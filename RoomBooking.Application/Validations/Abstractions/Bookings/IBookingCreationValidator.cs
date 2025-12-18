using RoomBooking.Core;
using RoomBooking.Core.Models;

namespace RoomBooking.Application.Validations.Abstractions.Bookings;

public interface IBookingCreationValidator
{
    Task<List<string>> Validate(
        Booking booking,
        CancellationToken cancellationToken = default);
}