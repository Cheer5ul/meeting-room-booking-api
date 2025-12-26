namespace RoomBooking.Application.DTOs.Room;

public class RoomUpdateDto
{
    public RoomUpdateDto(string name, int capacity, bool hasProjector,
        bool hasTv, bool hasWhiteBoard)
    {
        Name = name;
        Capacity = capacity;
        HasProjector = hasProjector;
        HasTv = hasTv;
        HasWhiteBoard = hasWhiteBoard;
    }
    
    public string Name { get; set; }
    public int Capacity { get; set; }
    public bool HasProjector { get; set; }
    public bool HasTv { get; set; }
    public bool HasWhiteBoard { get; set; }
}