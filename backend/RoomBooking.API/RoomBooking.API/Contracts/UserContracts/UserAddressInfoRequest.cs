namespace RoomBooking.API.Contracts.UserContracts;

public record UserAddressInfoRequest(
    string street, 
    string city, 
    string state, 
    string postalCode,
    string country
);