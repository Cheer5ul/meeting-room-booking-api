

namespace RoomBooking.DataAccess.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    
    public List<BookingEntity> Bookings { get; set; } = [];
    
}