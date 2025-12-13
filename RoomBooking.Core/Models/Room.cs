namespace RoomBooking.Core;

public class Room
{
    private Room(Guid id, string name, int capacity, 
        bool hasProjector, bool hasTv, bool hasWhiteBoard)
    {
        Id = id;
        Name = name;
        Capacity = capacity;
        HasProjector = hasProjector;
        HasTv = hasTv;
        HasWhiteBoard = hasWhiteBoard;
    }
    public Guid Id { get; }
    public string Name { get; } = string.Empty;
    public int Capacity { get; }
    public bool HasProjector { get; }
    public bool HasTv { get; }
    public bool HasWhiteBoard { get; }

    public static (Room room, string? error) Create(Guid id, string name, int capacity, 
        bool hasProjector, bool hasTv, bool hasWhiteBoard )
    {
        string error = string.Empty;
        if(string.IsNullOrEmpty(name))
            error = "Name cannot be empty";

        if (capacity <= 0)
            error = "Capacity is invalid";

        var room =  new Room(
            id,
            name,
            capacity,
            hasProjector,
            hasTv,
            hasWhiteBoard);
        
        return (room, error);
    }
}