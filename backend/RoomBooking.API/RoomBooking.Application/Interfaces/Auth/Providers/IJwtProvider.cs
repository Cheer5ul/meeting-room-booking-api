using RoomBooking.Core.Models.User;

namespace RoomBooking.Application.Interfaces.Auth.Providers;

public interface IJwtProvider
{
    string GenerateToken(User user);
}