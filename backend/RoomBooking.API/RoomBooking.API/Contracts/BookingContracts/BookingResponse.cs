namespace RoomBooking.API.Contracts.BookingContracts;

public record BookingResponse(
    Guid Id, 
    Guid RoomId,
    Guid UserId,
    DateTime StratTime,
    DateTime EndTime,
    string? Purpose);