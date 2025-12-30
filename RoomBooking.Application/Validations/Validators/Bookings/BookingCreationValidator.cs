using RoomBooking.Application.Validations.Abstractions.Bookings;
using RoomBooking.Core;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.Booking;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Validations.Validators.Bookings;

public class BookingCreationValidator : IBookingCreationValidator
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoomRepository _roomRepository;
    
    public BookingCreationValidator(IBookingRepository  bookingRepository, IUserRepository userRepository, IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _roomRepository = roomRepository;
    }

    public async Task<List<Error>> Validate(Booking booking, CancellationToken cancellationToken = default)
    {
        var errors = new List<Error>();
        if (booking.StartTime > booking.EndTime)
            errors.Add(BookingErrors.StartBeforeEndTime);
        
        if(booking.EndTime - booking.StartTime > TimeSpan.FromHours(6))
            errors.Add(BookingErrors.DurationExceeded);
        
        if(booking.StartTime < DateTime.UtcNow)
            errors.Add(BookingErrors.InThePast);

        if (booking.UserId == Guid.Empty ||
            booking.RoomId == Guid.Empty ||
            booking.UserId == booking.RoomId)
        {
            errors.Add(BookingErrors.InvalidIDs);
        }
        
        //Async overlaps check
        if (!errors.Any())
        {
            var hasConflict = await CheckForOverlaps(booking, cancellationToken);
            if(hasConflict)
                errors.Add(BookingErrors.IsOverlapping);
        }
        
        
        if (!errors.Any())
        {
            var userFor = await _userRepository.GetById(booking.UserId, cancellationToken);
            if (userFor == null || userFor.Id == Guid.Empty)
                errors.Add(BookingErrors.UserNotExisting);

            var roomFor = await _roomRepository.GetById(booking.RoomId, cancellationToken);
            if(roomFor == null || roomFor.Id == Guid.Empty)
                errors.Add(BookingErrors.RoomNotExisting);
        }

        return errors;
    }
    
    private async Task<bool> CheckForOverlaps(Booking booking, CancellationToken cancellationToken = default)
    {
        var existingItems = await _bookingRepository.GetByRoom(booking.RoomId, cancellationToken);
        
        return existingItems.Any(b => 
            booking.StartTime < b.EndTime && 
            b.StartTime < booking.EndTime);
    }
}