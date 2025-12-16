using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Core;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;

namespace RoomBooking.Application.Services;

public class UserService(
    IUserRepository userRepository, 
    IUserDeletionValidator deletionValidator,
    ILogger<UserService> logger) : IUserService
{
    private const string ServiceName = nameof(UserService);
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
    public async Task<List<User>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        const string methdodName = nameof(GetAllUsers);
        logger.LogInformation("----------------- {@RequestName}.{@Method} Started | {@DateTimeUtc} -----------------",
            ServiceName,
            methdodName,
            DateTime.UtcNow);
        
        try
        {
            var result = await userRepository.Get(cancellationToken);

            if (result == null || !result.Any())
            {
                logger.LogWarning("----------------- {Service}.{Method}: No users found | {@DatetimeUtc}----------------- ",
                    ServiceName,
                    methdodName,
                    DateTime.UtcNow);
                return new List<User>();
            }
            
            logger.LogInformation("----------------- {Service}.{Method}: completed. Found {@Count} | {@DatetimeUtc}----------------- ",
                ServiceName,
                methdodName,
                result.Count,
                DateTime.UtcNow);
            
            return result;
        }
        catch (Exception exception)
        {
            logger.LogError("----------------- {Service}.{Method}: failed with error {@ErrorMessage}) | {@DatetimeUtc}----------------- ", 
                ServiceName,
                methdodName,
                exception,
                DateTime.UtcNow);
            throw;
        }
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
        var validator = await deletionValidator.IsUserExists(id, cancellationToken);

        if (!validator)
        {
            //throwing exception for custom error middleware handler check, should use result pattern or error array
            throw new Exception("User does not exist");
        }
        
        return await userRepository.Delete(id, cancellationToken);
    }
    
}