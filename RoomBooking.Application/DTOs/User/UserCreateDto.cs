namespace RoomBooking.Application.DTOs.User;

public class UserCreateDto
{
    public UserCreateDto(string name, string email, string department, string password)
    {
        Name = name;
        Email = email;
        Department = department;
    }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Department { get; private set; }
    public string Password { get; private set; }
}