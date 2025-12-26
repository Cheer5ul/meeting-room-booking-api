namespace RoomBooking.Application.DTOs.User;

public class UserUpdateDto
{
    public UserUpdateDto(string name, string email, string department)
    {
        Name = name;
        Email = email;
        Department = department;
    }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
}