namespace RoomBooking.Core.Results.Errors;

public static class RoomErrors
{
    public static readonly Error RoomNotFound = new(
        "Room.RoomNotFound",
        "Room does not exist.");

}