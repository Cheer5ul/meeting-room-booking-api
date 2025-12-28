using System.Runtime.InteropServices.JavaScript;
using FluentValidation;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Models;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Validations.Validators.Bookings;

public sealed class BookingValidator : AbstractValidator<Booking>
{
    private readonly TimeSpan _maxDuration = TimeSpan.FromHours(6);
    private readonly TimeSpan _maxFutureBooking = TimeSpan.FromDays(365);
    
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoomRepository _roomRepository;
    
    public BookingValidator(
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _roomRepository = roomRepository;
        
        //Format and basic checks 
        RuleFor(booking => booking.StartTime)
            .NotEmpty()
            .WithMessage(BookingErrors.EmptyStartTime.Description)
            .WithErrorCode(BookingErrors.EmptyStartTime.Code)
            .Must(BeInFuture)
            .WithMessage(BookingErrors.InThePast.Description)
            .WithErrorCode(BookingErrors.InThePast.Code);

        //Not too late start check
        RuleFor(booking => booking.StartTime)
            .Must(StartsNotTooLate)
            .WithMessage(BookingErrors.StartsTooLate.Description)
            .WithErrorCode(BookingErrors.StartsTooLate.Code);
        
        RuleFor(b => b.EndTime)
            .NotEmpty()
            .WithMessage(BookingErrors.EmptyEndTime.Description)
            .WithErrorCode(BookingErrors.EmptyEndTime.Code)
            .GreaterThan(b => b.StartTime)
            .WithMessage(BookingErrors.StartBeforeEndTime.Description)
            .WithErrorCode(BookingErrors.StartBeforeEndTime.Code);
        
        //Duration check
        RuleFor(b => b)
            .Must(HaveValidDuration)
            .WithMessage(BookingErrors.DurationExceeded.Description)
            .WithErrorCode(BookingErrors.DurationExceeded.Code)
            .When(b => b.StartTime != default && b.EndTime != default);
        
        //IDs check
        RuleFor(b => b)
            .Must(ValidIDs)
            .WithMessage(BookingErrors.InvalidIDs.Description)
            .WithErrorCode(BookingErrors.InvalidIDs.Code);
        
        //Existing user and room checks 
        RuleFor(b => b)
            .MustAsync(async (booking, cancellationToken) => 
                await ExistingUserAndRoomAsync(booking, cancellationToken))
            .WithMessage(BookingErrors.UserOrRoomNotExisting.Description)
            .WithErrorCode(BookingErrors.UserOrRoomNotExisting.Code)
            .When(b => 
                b.StartTime != default &&  
                b.EndTime != default &&
                b.StartTime < b.EndTime);
        
        //Overlapping check
        RuleFor(b =>b)
            .MustAsync(async (booking, cancellationToken) =>
                !await HasOverlappingAsync(booking, cancellationToken)).
            WithMessage(BookingErrors.IsOverlapping.Description)
            .WithErrorCode(BookingErrors.IsOverlapping.Code)
            .When(b => b.RoomId != Guid.Empty &&  // avoiding unnecessary request to db
                       b.UserId != Guid.Empty && 
                       b.StartTime != default && // NRE protection
                       b.EndTime != default && // NRE protection
                       b.StartTime < b.EndTime); // avoiding unnecessary request with invalid times
    }

    private static bool BeInFuture(DateTime startTime)
    {
        return startTime > DateTime.UtcNow;
    }

    private bool HaveValidDuration(Booking booking)
    {
        if(booking.StartTime == default || booking.EndTime == default)
            return false;
        
        var duration = booking.EndTime - booking.StartTime;

        return duration <= _maxDuration;
    }

    private bool StartsNotTooLate(DateTime startTime)
    {
        //use utc
        var maxFutureBooking = DateTime.UtcNow + _maxFutureBooking;
        
        return startTime < maxFutureBooking;
    }

    private bool ValidIDs(Booking booking)
    {
        if(booking.UserId == Guid.Empty ||
           booking.RoomId == Guid.Empty ||
           booking.RoomId == booking.UserId)
            return false;
        
        return true;
    }

    private async Task<bool> ExistingUserAndRoomAsync(
        Booking booking, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetById(booking.UserId, cancellationToken) != null &&
               await _roomRepository.GetById(booking.RoomId, cancellationToken) != null;
    }
    
    private async Task<bool> HasOverlappingAsync(
        Booking booking, CancellationToken cancellationToken = default)
    {
        var existingItems = await _bookingRepository.GetByRoom(booking.RoomId, cancellationToken);
        
        //updating existing booking
        var bookingsToCheck = existingItems
            .Where(b => b.Id != booking.Id) //excluding the current one when updating
            .ToList();
        
        return bookingsToCheck.Any(b =>
            booking.StartTime < b.EndTime &&
            b.StartTime < booking.EndTime);
    }
}