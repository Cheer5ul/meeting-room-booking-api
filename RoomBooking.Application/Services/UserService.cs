using RoomBooking.Core;
using RoomBooking.Core.Abstractions;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;

namespace RoomBooking.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<List<User>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        return await userRepository.Get(cancellationToken);
    }

    public async Task<User?> GetUserById(Guid id, CancellationToken cancellationToken = default)
    {
        return await userRepository.GetById(id, cancellationToken);
    }

    public async Task<Guid> CreateUser(User user, CancellationToken cancellationToken = default)
    {
        return await userRepository.Create(user, cancellationToken);
    }

    public async Task<Guid> UpdateUser(Guid id, string name, string email, string department, 
        CancellationToken cancellationToken = default)
    {
        return await userRepository.Update(id, name, email, department, cancellationToken);
    }

    public async Task<Guid> DeleteUser(Guid id, CancellationToken cancellationToken = default)
    {
        return await userRepository.Delete(id, cancellationToken);
    }
}