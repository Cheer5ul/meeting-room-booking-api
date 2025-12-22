using Microsoft.Extensions.Logging;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models;


namespace RoomBooking.Application.Services;

public class UserService(
    IUserRepository userRepository, 
    IUserDeletionValidator deletionValidator,
    ILogger<UserService> logger) : IUserService
{
    public async Task<List<User>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        return await userRepository.Get(cancellationToken);
    }

    public async Task<User?> GetUserById(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("{@MethodName}: Getting user by id: {@UserId}", nameof(GetUserById), id);
        var user =  await userRepository.GetById(id, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User not found: {@UserId}", id);
        }
        return user;
    }

    public async Task<Guid> CreateUser(User user, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Creating new user. Name: {@UserName}, Email: {@UserEmail}",
            nameof(CreateUser), user.Name, user.Email);

        try
        {
           var userId = await userRepository.Create(user, cancellationToken);
           
           logger.LogInformation("{@MethodName}: User created successfully: UserId: {@UserId}", 
               nameof(CreateUser), userId);
           
           return userId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{@MethodName}: Error creating user: {@UserName}, Email: {@UserEmail}", 
                nameof(CreateUser),
                user.Name,
                user.Email);
            throw;
        }
    }

    public async Task<Guid> UpdateUser(Guid id, string name, string email, string department, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Updating user {@UserId}. New values - Name: {@UserName}, Department: {@Department}", 
            nameof(UpdateUser), id, name, department);
        
        return await userRepository.Update(id, name, email, department, cancellationToken);
    }

    public async Task<Guid> DeleteUser(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Attempting to delete user {@UserId}",
            nameof(DeleteUser), id);
        var canDelete = await deletionValidator.IsUserExists(id, cancellationToken);

        if (!canDelete)
        {
            logger.LogWarning("{@MethodName}: Cannot delete unexisting user: {@UserId}",
                nameof(DeleteUser), id);
            throw new Exception("User does not exist");
        }

        try
        {
            var deletedId = await userRepository.Delete(id, cancellationToken);
            logger.LogInformation("{@MethodName}: User was successfully deleted: {@UserId}",
                nameof(DeleteUser), deletedId);
            return deletedId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{@MethodName}: Error deleting user: {@UserId}",
                nameof(DeleteUser),id);
            throw;
        }
    }
    
}