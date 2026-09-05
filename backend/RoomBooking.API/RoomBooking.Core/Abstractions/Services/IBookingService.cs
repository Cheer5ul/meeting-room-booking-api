using RoomBooking.Core.Models;
using RoomBooking.Core.Models.Booking;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Core.Abstractions.Services;

public interface IBookingService
{
    Task<Result<List<Booking>>> GetAllBookings(CancellationToken cancellationToken = default);
    Task<Result<List<Booking>>> GetByRoom(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<Booking>>> GetByUser(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Guid>> Create(Booking booking, CancellationToken cancellationToken = default);
    Task<Result<Guid>> Delete(Guid id, CancellationToken cancellationToken = default);
}