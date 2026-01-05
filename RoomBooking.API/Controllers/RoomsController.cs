using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomBooking.API.Contracts.RoomContracts;
using RoomBooking.API.FailureHandlers;
using RoomBooking.Core;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.Room;

namespace RoomBooking.API.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IFailureHandler _failureHandler;

    public RoomsController(IRoomService roomService, IFailureHandler failureHandler)
    {
        _roomService = roomService;
        _failureHandler = failureHandler;
    }

    [HttpGet]
    [EnableRateLimiting("token-by-ip")]
    public async Task<ActionResult<List<RoomResponse>>> GetAllRooms(CancellationToken cancellationToken)
    {
        var result = await _roomService.GetAllRooms(cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }
        
        var successfulRooms = result.Value!;
        
        var response = successfulRooms.Select(r => new RoomResponse(
            r.Id,
            r.Name,
            r.Capacity,
            r.HasProjector,
            r.HasTv,
            r.HasWhiteBoard));

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [EnableRateLimiting("fixed-by-ip")] 
    public async Task<ActionResult<RoomResponse>> GetRoomById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roomService.GetRoomById(id, cancellationToken);

        if (result.IsFailure)
        {
            return  _failureHandler.HandleFailure(result, HttpContext);
        }

        var successfulRoom = result.Value!;
        
        var response = new RoomResponse(
            successfulRoom.Id,
            successfulRoom.Name,
            successfulRoom.Capacity,
            successfulRoom.HasProjector,
            successfulRoom.HasTv,
            successfulRoom.HasWhiteBoard);

        return Ok(response);
    }

    [HttpPost]
    [EnableRateLimiting("fixed-by-ip")]
    public async Task<ActionResult<Guid>> CreateRoon([FromBody] RoomRequest roomRequest,
        CancellationToken cancellationToken)
    {
        var (room, error) = Room.Create(
            Guid.NewGuid(),
            roomRequest.Name,
            roomRequest.Capacity,
            roomRequest.HasProjector,
            roomRequest.HasTv,
            roomRequest.HasWhiteBoard);

        if (!string.IsNullOrEmpty(error))
        {    //think over a better way and logging
            return BadRequest(error);
        }
        
        var result = await _roomService.CreateRoom(room, cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }

        var successfulRoom = result.Value!;
        
        return Ok(successfulRoom);
    }

    [HttpPut("{id:guid}")]
    [EnableRateLimiting("fixed-by-ip")]
    public async Task<ActionResult<Guid>> UpdateRoom(Guid id, [FromBody] RoomRequest roomRequest,
        CancellationToken cancellationToken)
    {
        var result = await _roomService.UpdateRoom(
            id, 
            roomRequest.Name, 
            roomRequest.Capacity,
            roomRequest.HasProjector,
            roomRequest.HasTv,
            roomRequest.HasWhiteBoard,
            cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }

        var successfulRoom = result.Value!;
        
        //Not returning id
        var rowsAffected = Enumerable
            .Range(0, successfulRoom.Length)
            .Where(i => successfulRoom[i]?.GetType() != typeof(Guid))
            .Select(i => successfulRoom[i]?.ToString())
            .ToList();
        
        return Ok(rowsAffected);
    }

    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("fixed-by-ip")]
    public async Task<ActionResult<Guid>> DeleteRoom(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roomService.DeleteRoom(id, cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }

        var successfulRoom = result.Value!;
        
        return Ok(successfulRoom);
    }

}