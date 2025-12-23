namespace RoomBooking.API.Contracts.UserContracts;

public record UserRequest(
    string Name,
    string Email,
    string Department
 );