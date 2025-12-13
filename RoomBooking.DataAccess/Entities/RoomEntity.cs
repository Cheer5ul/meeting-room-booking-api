namespace RoomBooking.DataAccess.Entities;

public class RoomEntity
{
    public Guid Id { get; set;}
    public string Name { get; set;}  = string.Empty;
    public int Capacity { get; set;}
    public bool HasProjector { get; set;}
    public bool HasTv { get; set;}
    public bool HasWhiteBoard { get; set;}
    
    public List<BookingEntity> Bookings { get; set;} = [];
}