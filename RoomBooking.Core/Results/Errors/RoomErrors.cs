namespace RoomBooking.Core.Results.Errors;

public static class RoomErrors
{
    public static readonly Error RoomNotFound = new(
        "Room.RoomNotFound",
        "Room does not exist.");
    
    public static readonly Error NameRequired = new(
        "Room.NameRequired",
        "Name is required.");

    public static readonly Error CapacityGreaterThanZero = new(
        "Room.CapacityGreaterThanZero",
        "Capacity has to be greater than zero.");
    
    public static readonly Error NameTooLong = new(
        "Room.NameTooLong",
        "Room name is too long.");
    
    public static readonly Error CapacityRequired = new(
        "Room.CapacityRequired",
        "Capacity is required.");
    
    public static readonly Error ProjectorInfoRequired = new (
        "Room.ProjectorInfoRequired",
        "Projector info is required.");

    public static readonly Error TvInfoRequired = new(
        "Room.TvInfoRequired",
        "TV info is required.");

    public static readonly Error WhiteBoardInfoRequired = new(
        "Room.WhiteBoardInfoRequired",
        "White board info is required.");

}