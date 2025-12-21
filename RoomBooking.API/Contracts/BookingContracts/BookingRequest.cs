namespace RoomBooking.API.Contracts.BookingContracts;

public record BookingRequest(
    Guid RoomId,
    Guid UserId,
    DateTime StartTime,
    DateTime EndTime,
    string? Purpose = null);