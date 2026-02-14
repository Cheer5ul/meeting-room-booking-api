using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.Extensions.Logging;
using RoomBooking.Application.DTOs.Room;
using RoomBooking.Application.Validations.Abstractions.Rooms;
using RoomBooking.Application.Validations.Abstractions.Validators;
using RoomBooking.Application.Validations.Validators.Rooms;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.Room;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Services;

public class RoomService(IRoomRepository repository,
    IRoomGettingValidator validator,
    IValidator<Room> roomCreationValidator,
    IValidator<RoomUpdateDto> roomUpdateDtoValidator,
    ILogger<RoomService> logger,
    IValidationToErrorConverter toErrorConverter) : IRoomService
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
        
        //edit: some validation done ; fix controller validation!
        var validationResult = roomCreationValidator.Validate(instance: room);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("{@MethodName} Validation errors occured while creating a new room: {@Erros}",
                nameof(CreateRoom), validationResult.Errors);
            
            var errors = toErrorConverter.ValidationToErrors(validationResult.Errors);

            return Result<Guid>.Failures(errors);
        }
        
        var result = await repository.Create(room, cancellationToken);
            
        logger.LogInformation("{@MethodName}: Room created successfully: {@Room}",
            nameof(CreateRoom), result);
        return result;
    }

    public async Task<Result<ITuple>> UpdateRoom(Guid id, string name, int capacity, bool hasProjector,
        bool hasTv, bool hasWhiteBoard, CancellationToken cancellationToken = default)
    {
        var canUpdate = await validator.IsExisting(id, cancellationToken);
        
        var roomUpdateDto = new RoomUpdateDto(name, capacity, hasProjector, hasTv, hasWhiteBoard);
        
        var validationResult = roomUpdateDtoValidator.Validate(roomUpdateDto);
        
        if (!canUpdate ||  !validationResult.IsValid)
        {
            logger.LogInformation("{@MethodName}: Room updating failed: {@Room}",
                nameof(UpdateRoom), id);
            
            var errors = toErrorConverter.ValidationToErrors(validationResult.Errors);

            if (!canUpdate)
            {
                logger.LogInformation("{@MethodName} Room {@RoomId} does not exist",
                    nameof(UpdateRoom), id);
                errors.Add(RoomErrors.RoomNotFound);
            }
            
            return Result<ITuple>.Failures(errors);
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
            return Result<Guid>.Failures([RoomErrors.RoomNotFound]);
        }
        
        var deletedRows = await repository.Delete(id, cancellationToken);
        
        logger.LogInformation("{@MethodName}: Room deleted successfully: {@Room}",
            nameof(DeleteRoom), deletedRows);
        return deletedRows;
    }
}