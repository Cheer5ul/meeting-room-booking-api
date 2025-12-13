namespace RoomBooking.Core.Abstractions.Repositories;

public interface IRoomRepository
{
    Task<List<Room>> Get(CancellationToken cancellationToken = default);
    Task<Room?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> Create(Room room, CancellationToken cancellationToken = default);

    Task<Guid> Update(Guid id, string name, int capacity, bool hasProjector,
        bool hasTv, bool hasWhiteBoard, CancellationToken cancellationToken = default);

    Task<Guid> Delete(Guid id, CancellationToken cancellationToken = default);
}