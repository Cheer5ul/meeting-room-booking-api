using RoomBooking.Core;
using RoomBooking.Core.Models;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Validations.Abstractions.Bookings;

public interface IBookingCreationValidator
{
    Task<List<Error>> Validate(
        Booking booking,
        CancellationToken cancellationToken = default);
}