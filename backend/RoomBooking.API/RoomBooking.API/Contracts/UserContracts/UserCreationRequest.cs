namespace RoomBooking.API.Contracts.UserContracts;

public record UserCreationRequest(
    string Name,
    string Email,
    string Department,
    string Password
 );