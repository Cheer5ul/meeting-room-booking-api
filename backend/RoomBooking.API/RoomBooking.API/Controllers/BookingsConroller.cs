using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomBooking.API.Contracts.BookingContracts;
using RoomBooking.API.FailureHandlers;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.Booking;


namespace RoomBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IFailureHandler _failureHandler;
    public BookingController(
        IBookingService bookingService, 
        IFailureHandler failureHandler)
    {
        _bookingService = bookingService;
        _failureHandler = failureHandler;
    }

    [HttpGet]
    [EnableRateLimiting("token-by-ip")]
    public async Task<ActionResult<List<BookingResponse>>> GetAllBookings(CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetAllBookings(cancellationToken);
        
        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }
        
        var successfulBookings = result.Value!;

        var response = successfulBookings.Select(b => new BookingResponse(
            b.Id,
            b.RoomId,
            b.UserId,
            b.StartTime,
            b.EndTime,
            b.Purpose));
        
        
        return Ok(response);
    }

    [HttpGet("by-user/{userId:guid}")]
    [EnableRateLimiting("fixed-by-ip")] 
    public async Task<ActionResult<BookingResponse>> GetBookingByUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetByUser(userId, cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }

        var successfulBookings = result.Value!;
        
        var response = successfulBookings.Select(b => new BookingResponse(
            b.Id,
            b.RoomId,
            b.UserId,
            b.StartTime,
            b.EndTime,
            b.Purpose));
        
        return Ok(response);
    }
    
    [HttpGet("by-room/{roomId:guid}")]
    [EnableRateLimiting("fixed-by-ip")] 
    public async Task<ActionResult<BookingResponse>> GetBookingByRoom(Guid roomId, CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetByRoom(roomId, cancellationToken);
        
        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }
        
        var successfulBookings = result.Value!;
        
        var response = successfulBookings.Select(b => new BookingResponse(
            b.Id,
            b.RoomId,
            b.UserId,
            b.StartTime,
            b.EndTime,
            b.Purpose));
        
        return Ok(response);
    }

    [HttpPost]
    [EnableRateLimiting("fixed-by-ip")] 
    public async Task<ActionResult<Guid>> CreateBooking([FromBody] BookingRequest bookingRequest,
        CancellationToken cancellationToken)
        {
        var booking = Booking.Create(
            Guid.NewGuid(),
            bookingRequest.RoomId,
            bookingRequest.UserId,
            bookingRequest.StartTime,
            bookingRequest.EndTime,
            bookingRequest.Purpose);
        
        var result = await _bookingService.Create(booking.booking, cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }
        
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("fixed-by-ip")] 
    public async Task<ActionResult<Guid>> DeleteBooking(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bookingService.Delete(id, cancellationToken);

        if (result.IsFailure)
        {
            return _failureHandler.HandleFailure(result, HttpContext);
        }
        
        return Ok(result.Value);
    }
    
    
}