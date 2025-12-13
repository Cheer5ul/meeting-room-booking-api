using RoomBooking.Core;
using RoomBooking.Core.Abstractions;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;

namespace RoomBooking.Application.Services;

public class RoomService(IRoomRepository repository) : IRoomService
{
    public async Task<List<Room>> GetAllRooms(CancellationToken cancellationToken = default)
    {
        return await repository.Get(cancellationToken);
    }

    public async Task<Room?> GetRoomById(Guid id, CancellationToken cancellationToken = default)
    {
        return await repository.GetById(id, cancellationToken);
    }

    public async Task<Guid> CreateRoom(Room room, CancellationToken cancellationToken = default)
    {
        return await repository.Create(room, cancellationToken);
    }

    public async Task<Guid> UpdateRoom(Guid id, string name, int capacity, bool hasProjector,
        bool hasTv, bool hasWhiteBoard, CancellationToken cancellationToken = default)
    {
        return await repository.Update(id, name, capacity, hasProjector, hasTv, hasWhiteBoard, cancellationToken);
    }

    public async Task<Guid> DeleteRoom(Guid id, CancellationToken cancellationToken = default)
    {
        return await repository.Delete(id, cancellationToken);
    }
}