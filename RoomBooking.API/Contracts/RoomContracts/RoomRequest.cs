namespace RoomBooking.API.Contracts.RoomContracts;

public record RoomRequest(
    string Name,
    int Capacity,
    bool HasProjector,
    bool HasTv,
    bool HasWhiteBoard);