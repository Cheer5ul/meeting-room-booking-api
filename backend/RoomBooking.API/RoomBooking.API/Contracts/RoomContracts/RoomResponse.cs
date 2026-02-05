namespace RoomBooking.API.Contracts.RoomContracts;

public record RoomResponse(
    Guid Id, 
    string Name, 
    int Capacity, 
    bool HasProjector, 
    bool HasTv, 
    bool HasWhiteBoard);