using RoomBooking.Application.Interfaces.Auth.Hashers;

namespace RoomBooking.Infrastructure.Hashers;

public class UserPasswordHasher : IUserPasswordHasher
{
    public string Generate(string password)
    {
        string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        
        return hashedPassword;
    }

    public bool Verify(string password, string hashedPassword)
    {
        bool isMatching = BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
        return isMatching;
    }
}