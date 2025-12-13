namespace RoomBooking.Core.Abstractions.Repositories;

public interface IBookingRepository
{
    Task<List<Booking>> Get(CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByRoom(Guid id, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByUser(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> Create(Booking booking, CancellationToken cancellationToken = default);
    Task<Guid> Delete(Guid id, CancellationToken cancellationToken = default);
}