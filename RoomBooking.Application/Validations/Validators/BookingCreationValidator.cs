using RoomBooking.Application.Validations.Abstractions;
using RoomBooking.Core;
using RoomBooking.Core.Abstractions;
using RoomBooking.Core.Abstractions.Repositories;

namespace RoomBooking.Application.Validations.Validators;

public class BookingCreationValidator : IBookingCreationValidator
{
    private readonly IBookingRepository _repository;
    
    public BookingCreationValidator(IBookingRepository  repository)
    {
        _repository = repository;
    }

    public async Task<List<string>> Validate(Booking booking, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();
        if (booking.StartTime > booking.EndTime)
            errors.Add("Start time must be before end time.");
        
        if(booking.EndTime -  booking.StartTime < TimeSpan.FromHours(6))
            errors.Add("Booking duration cannot exceed 6 hours.");
        
        if(booking.StartTime < DateTime.UtcNow)
            errors.Add("Booking cannot be in the past.");
        
        //Async overlaps check
        if (!errors.Any())
        {
            var hasConflict = await CheckForOverlaps(booking, cancellationToken);
            if(hasConflict)
                errors.Add("Room is already booked for this time");
        }

        return errors;
    }
    
    public async Task<bool> CheckForOverlaps(Booking booking, CancellationToken cancellationToken = default)
    {
        var existingItems = await _repository.GetByRoom(booking.RoomId, cancellationToken);
        
        return existingItems.Any(b => 
            booking.StartTime < b.EndTime && 
            b.StartTime < booking.EndTime);
    }
}