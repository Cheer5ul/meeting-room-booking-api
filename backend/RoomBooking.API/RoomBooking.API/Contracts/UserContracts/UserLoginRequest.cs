namespace RoomBooking.API.Contracts.UserContracts;

public record UserLoginRequest(
    string Email,
    string Password
);