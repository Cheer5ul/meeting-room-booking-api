using RoomBooking.Core.Models;

namespace RoomBooking.Core.Abstractions.Services;

public interface IBookingService
{
    Task<List<Booking>> GetAllBookings(CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByRoom(Guid id, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByUser(Guid id, CancellationToken cancellationToken = default);
    Task<(Guid? Guid, List<string> Errors)> Create(Booking booking, CancellationToken cancellationToken = default);
    Task<(Guid? Guid, List<string> Errors)> Delete(Guid id, CancellationToken cancellationToken = default);
}