namespace RoomBooking.API.Contracts.UserContracts;

public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    string Department
);
    