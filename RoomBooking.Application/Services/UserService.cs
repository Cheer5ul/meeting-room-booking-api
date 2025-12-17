using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Core;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Application.Services;


namespace RoomBooking.Application.Services;

public class UserService(
    IUserRepository userRepository, 
    IUserDeletionValidator deletionValidator,
    IServiceLogger<User> logger) : IUserService
{
    private const string ServiceName = nameof(UserService);
    public async Task<List<User>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await userRepository.Get(cancellationToken);
            logger.LogResultCollection(result, ServiceName, nameof(GetAllUsers));
            return result;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, ServiceName, nameof(GetAllUsers));
            throw;
        }
        
    }

    public async Task<User?> GetUserById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result =  await userRepository.GetById(id, cancellationToken);
            logger.LogResult(result, ServiceName, nameof(GetUserById));
            return result;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, ServiceName, nameof(GetAllUsers));
            throw;
        }
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
        var validator = await deletionValidator.IsUserExists(id, cancellationToken);

        if (!validator)
        {
            //throwing exception for custom error middleware handler check, should use result pattern or error array
            throw new Exception("User does not exist");
        }
        
        return await userRepository.Delete(id, cancellationToken);
    }
    
}