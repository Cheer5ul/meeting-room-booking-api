namespace RoomBooking.API.Contracts.BookingContracts;

public record BookingRequest(
    Guid RoomId,
    Guid UserId,
    DateTime StratTime,
    DateTime EndTime,
    string? Purpose);