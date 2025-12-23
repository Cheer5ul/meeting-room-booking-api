

namespace RoomBooking.DataAccess.Entities.UserEntity;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    
    public List<BookingEntity.BookingEntity> Bookings { get; set; } = [];
    
    public AddressInfoEntity? AddressInfo { get; set;}
    
}