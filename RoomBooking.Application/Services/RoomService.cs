using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using RoomBooking.Application.Validations.Abstractions.Rooms;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Services;

public class RoomService(IRoomRepository repository,
    IRoomGettingValidator validator,
    ILogger<RoomService> logger) : IRoomService
{
    public async Task<Result<List<Room>>> GetAllRooms(CancellationToken cancellationToken = default)
    {
        return await repository.Get(cancellationToken);
    }

    public async Task<Result<Room?>> GetRoomById(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Getting room with by {@Id}",
            nameof(GetRoomById), id);
        
        var room = await repository.GetById(id, cancellationToken);

        if (room == null)
        {
            logger.LogInformation("Room not found: {@RoomId}",
                id);
            return Result<Room?>.Failures([RoomErrors.RoomNotFound]);
        }

        return room;
    }

    public async Task<Result<Guid>> CreateRoom(Room room, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Creating a new room: {@Room}",
            nameof(CreateRoom), room);
        
        //need some validation and fix controller validation
        
        var result = await repository.Create(room, cancellationToken);
            
        logger.LogInformation("{@MethodName}: Room created successfully: {@Room}",
            nameof(CreateRoom), result);
        return result;
    }

    public async Task<Result<ITuple>> UpdateRoom(Guid id, string name, int capacity, bool hasProjector,
        bool hasTv, bool hasWhiteBoard, CancellationToken cancellationToken = default)
    {
        var canUpdate = await validator.IsExisting(id, cancellationToken);
        if (!canUpdate)
        {
            logger.LogInformation("{@MethodName}: Cannot update unexisting room: {@Room}",
                nameof(UpdateRoom), id);
            return Result<ITuple>.Failures([RoomErrors.RoomNotFound]);
        }

        var affectedRows = await repository.Update(id, name, capacity, hasProjector, hasTv, hasWhiteBoard, cancellationToken);
        
        logger.LogInformation("{@MethodName}: Room updated successfully: {@Room}",
            nameof(UpdateRoom), affectedRows);

        return Result<ITuple>.Success(affectedRows);
    }

    public async Task<Result<Guid>> DeleteRoom(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Attempting to delete room {@UserId}",
            nameof(DeleteRoom), id);
        
        var canDelete = await validator.IsExisting(id, cancellationToken);
        if (!canDelete)
        {
            logger.LogInformation("{@MethodName}: Cannot delete unexisting room: {@Room}",
                nameof(UpdateRoom), id);
            return Result<Guid>.Failures([UserErrors.UserNotFound]);
        }
        
        var deletedRows = await repository.Delete(id, cancellationToken);
        
        logger.LogInformation("{@MethodName}: Room deleted successfully: {@Room}",
            nameof(DeleteRoom), deletedRows);
        return deletedRows;
    }
}