using Microsoft.AspNetCore.Mvc;
using RoomBooking.API.Contracts.RoomContracts;
using RoomBooking.Core;
using RoomBooking.Core.Abstractions.Services;

namespace RoomBooking.API.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomResponse>>> GetAllRooms(CancellationToken cancellationToken)
    {
        var rooms = await _roomService.GetAllRooms(cancellationToken);

        var response = rooms.Select(r => new RoomResponse(
            r.Id,
            r.Name,
            r.Capacity,
            r.HasProjector,
            r.HasTv,
            r.HasWhiteBoard));

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomResponse>> GetRoomById(Guid id, CancellationToken cancellationToken)
    {
        var room = await _roomService.GetRoomById(id, cancellationToken);

        if (room == null)
            return NotFound();
        
        var response = new RoomResponse(
            room.Id,
            room.Name,
            room.Capacity,
            room.HasProjector,
            room.HasTv,
            room.HasWhiteBoard);

        return Ok(response);
    }

    [HttpPost]
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
        {
            return BadRequest(error);
        }
        
        var roomId = await _roomService.CreateRoom(room, cancellationToken);

        return Ok(roomId);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> UpdateRoom(Guid id, [FromBody] RoomRequest roomRequest,
        CancellationToken cancellationToken)
    {
        var roomId = await _roomService.UpdateRoom(
            id, 
            roomRequest.Name, 
            roomRequest.Capacity,
            roomRequest.HasProjector,
            roomRequest.HasTv,
            roomRequest.HasWhiteBoard,
            cancellationToken);
        
        return Ok(roomId);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteRoom(Guid id, CancellationToken cancellationToken)
    {
        var roomId = await _roomService.DeleteRoom(id, cancellationToken);
        
        return Ok(roomId);
    }

}