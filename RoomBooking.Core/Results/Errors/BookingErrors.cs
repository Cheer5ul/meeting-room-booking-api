namespace RoomBooking.Core.Results.Errors;

public static class BookingErrors
{
    public static readonly Error StartBeforeEndTime = new Error(
        "Booking.StartBeforeEndTime",
        "Start time must be before end time.");

    public static readonly Error DurationExceeded = new Error(
        "Booking.DurationExceeded",
        "Booking duration cannot exceed 6 hours.");

    public static readonly Error IsOverlapping = new Error(
        "Booking.IsOverlapping",
        "Room is already booked for this time.");

    public static readonly Error InThePast = new Error(
        "Booking.InThePast",
        "Booking cannot be in the past.");
    
    public static readonly Error NotFound = new Error(
        "Booking.NotFound",
        "Booking not found");
    
    public static readonly Error InvalidIDs = new Error(
        "Booking.InvalidIDs",
        "Booking IDs are invalid.");
    
    public static readonly Error RoomNotExisting = new Error(
        "Booking.RoomNotExist",
        "Room you are trying to book doesn't exist.");
    
    public static readonly Error UserNotExisting = new Error(
        "Booking.UserNotExist",
        "User you are booking for doesn't exist.");
    
    public static readonly Error ValidationFailed = new Error(
        "Booking.ValidationFailed",
        
        "Validation failed.");
    public static readonly Error Conflict = new Error(
        "Booking.Conflict",
        "Conflict error occurred.");
    
    public static readonly Error Forbidden = new Error(
        "Booking.Forbidden",
        "Forbidden error occurred.");
}