using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RoomBooking.API.Contracts.BookingContracts;
using RoomBooking.API.FailureHandlers;
using RoomBooking.Application.Services;
using RoomBooking.Core;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models;
using RoomBooking.Core.Results;


namespace RoomBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly FailureHandler _failureHandler;
    public BookingController(IBookingService bookingService, FailureHandler failureHandler)
    {
        _bookingService = bookingService;
        _failureHandler = failureHandler;
    }

    [HttpGet]
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
        var booking = Booking.Create(
            Guid.NewGuid(),
            bookingRequest.RoomId,
            bookingRequest.UserId,
            bookingRequest.StartTime,
            bookingRequest.EndTime,
            bookingRequest.Purpose);
        
        var bookingId = await _bookingService.Create(booking.booking, cancellationToken);

        if (bookingId.Errors.Any())
        {
            var modelState = new ModelStateDictionary();

            var errors = bookingId.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key.ToLowerInvariant(),
                    g => g.Select(e => e.Description).ToArray()
                    );
            
            var validationProblem = new ValidationProblemDetails(modelState)
            {
                Type = "https://example.com/errors/validation",
                Title = "Validation Error",
                Status = 400,
                Detail = "Please correct the specified errors and try again.",
                Instance = HttpContext.Request.Path
            };
            
            validationProblem.Extensions.Add("errors", bookingId.Errors);
            
            return BadRequest(validationProblem);
        }
        
        return Ok(bookingId.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteBooking(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bookingService.Delete(id, cancellationToken);

        if (result.Errors.Any())
        {
            return BadRequest(result.Errors);
        }
        
        return Ok(result.Value);
    }
    
    
}