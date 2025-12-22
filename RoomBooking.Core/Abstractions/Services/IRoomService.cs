using System.Runtime.CompilerServices;
using RoomBooking.Core.Models;
using RoomBooking.Core.Results;

namespace RoomBooking.Core.Abstractions.Services;

public interface IRoomService
{
    Task<Result<List<Room>>> GetAllRooms(CancellationToken cancellationToken = default);
    Task<Result<Room?>> GetRoomById(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateRoom(Room room, CancellationToken cancellationToken = default);

    Task<Result<ITuple>> UpdateRoom(Guid id, string name, int capacity, bool hasProjector,
        bool hasTv, bool hasWhiteBoard, CancellationToken cancellationToken = default);

    Task<Result<Guid>> DeleteRoom(Guid id, CancellationToken cancellationToken = default);
}