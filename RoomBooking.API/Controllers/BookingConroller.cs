using Microsoft.AspNetCore.Mvc;
using RoomBooking.API.Contracts.BookingContracts;
using RoomBooking.Application.Services;
using RoomBooking.Core;


namespace RoomBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookingResponse>>> GetAllBookings(CancellationToken cancellationToken)
    {
        var bookings = await _bookingService.GetAllBookings(cancellationToken);

        var response = bookings.Select(b => new BookingResponse(
            b.Id,
            b.RoomId,
            b.UserId,
            b.StartTime,
            b.EndTime,
            b.Purpose));
        
        return Ok(response);
    }

    [HttpGet("by-user/{userId:guid}")]
    public async Task<ActionResult<BookingResponse>> GetBookingByUser(Guid userId, CancellationToken cancellationToken)
    {
        var bookings = await _bookingService.GetByUser(userId, cancellationToken);
        
        var response = bookings.Select(b => new BookingResponse(
            b.Id,
            b.RoomId,
            b.UserId,
            b.StartTime,
            b.EndTime,
            b.Purpose));
        
        return Ok(response);
    }
    
    [HttpGet("by-room/{roomId:guid}")]
    public async Task<ActionResult<BookingResponse>> GetBookingByRoom(Guid roomId, CancellationToken cancellationToken)
    {
        var bookings = await _bookingService.GetByRoom(roomId, cancellationToken);
        
        var response = bookings.Select(b => new BookingResponse(
            b.Id,
            b.RoomId,
            b.UserId,
            b.StartTime,
            b.EndTime,
            b.Purpose));
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateBooking([FromBody] BookingRequest bookingRequest,
        CancellationToken cancellationToken)
    {
        var (booking, error) = Booking.Create(
            Guid.NewGuid(),
            bookingRequest.RoomId,
            bookingRequest.UserId,
            DateTime.UtcNow,
            bookingRequest.EndTime,
            bookingRequest.Purpose);

        if (!string.IsNullOrEmpty(error))
        {
            return BadRequest(error);
        }
        
        var bookingId = await _bookingService.Create(booking, cancellationToken);

        if (bookingId.Errors.Any())
        {
            return BadRequest(bookingId.Errors);
        }
        
        return Ok(bookingId);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteBooking(Guid id, CancellationToken cancellationToken)
    {
        var bookingId = await _bookingService.Delete(id, cancellationToken);
        
        return Ok(bookingId);
    }
}