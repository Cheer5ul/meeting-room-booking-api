using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.User;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;


namespace RoomBooking.Application.Services;

public class UserService(
    IUserRepository userRepository, 
    IUserGettingValidator userValidator,
    ILogger<UserService> logger) : IUserService
{
    public async Task<Result<List<User>>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        return await userRepository.Get(cancellationToken);
    }

    public async Task<Result<User?>> GetUserById(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Getting user by id: {@UserId}", nameof(GetUserById), id);
        var user = await userRepository.GetById(id, cancellationToken);

        if (user == null)
        {
            logger.LogInformation("User not found: {@UserId}", id);
            return Result<User?>.Failures([UserErrors.UserNotFound]);
        }
        return user;
    }

    public async Task<Result<Guid>> CreateUser(User user, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Creating new user. Name: {@UserName}, Email: {@UserEmail}",
            nameof(CreateUser), user.Name, user.Email);
        
        //have to add validation here
        
       // if (errors.Any())
       // {
       //     logger.LogError("{@MethodName}: Error creating user: {@UserName}, Email: {@UserEmail}", 
       //         nameof(CreateUser),
       //         user.Name,
       //         user.Email);
       //     //return Result<Guid>.Failures(errors:);
       // }
       
       var userId = await userRepository.Create(user, cancellationToken);
       logger.LogInformation("{@MethodName}: User created successfully: UserId: {@UserId}", 
           nameof(CreateUser), userId);
       return userId;
    }

    public async Task<Result<ITuple>> UpdateUser(Guid id, string name, string email, string department, 
        CancellationToken cancellationToken = default)
    {
        var canUpdate = await userValidator.IsUserExists(id, cancellationToken);

        if (!canUpdate)
        {
            logger.LogWarning("{@MethodName}: Cannot update unexisting user: {@UserId}",
                nameof(UpdateUser), id);
            return Result<ITuple>.Failures([UserErrors.UserNotFound]);
        }
        
        var affectedRows = await userRepository.Update(id, name, email, department, cancellationToken);

        logger.LogInformation("{@MethodName}: Updating user {@UserId}. New values - Name: {@UserName}, Department: {@Department}", 
            nameof(UpdateUser), id, name, department);

        return Result<ITuple>.Success(affectedRows);
    }

    public async Task<Result<Guid>> DeleteUser(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Attempting to delete user {@UserId}",
            nameof(DeleteUser), id);
        
        var canDelete = await userValidator.IsUserExists(id, cancellationToken);
        if (!canDelete)
        {
            logger.LogWarning("{@MethodName}: Cannot delete unexisting user: {@UserId}",
                nameof(DeleteUser), id);
            return Result<Guid>.Failures([UserErrors.UserNotFound]);
        }
        
        var deletedId = await userRepository.Delete(id, cancellationToken);
        
        logger.LogInformation("{@MethodName}: User was successfully deleted: {@UserId}",
            nameof(DeleteUser), deletedId);
        return deletedId;
    }

    public async Task<Result<ITuple>> AddAddressInfo(
        Guid id, string street, string city, string state, string postalCode, string country,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Attempting to add AddressInfo for user {@UserId}",
            nameof(AddAddressInfo), id);

        var addedInfo = await userRepository.AddAddressInfo(
            id, street, city, state, postalCode, country, cancellationToken);

        return Result<ITuple>.Success(addedInfo);
    }
}