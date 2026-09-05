namespace RoomBooking.Application.Interfaces.Auth.Hashers;

public interface IUserPasswordHasher
{
    string Generate(string password);
    bool Verify(string password, string hashedPassword);
}