namespace RoomBooking.Core.Abstractions.Services;

public interface IRoomService
{
    Task<List<Room>> GetAllRooms(CancellationToken cancellationToken = default);
    Task<Room?> GetRoomById(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateRoom(Room room, CancellationToken cancellationToken = default);

    Task<Guid> UpdateRoom(Guid id, string name, int capacity, bool hasProjector,
        bool hasTv, bool hasWhiteBoard, CancellationToken cancellationToken = default);

    Task<Guid> DeleteRoom(Guid id, CancellationToken cancellationToken = default);
}