using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.Extensions.Logging;
using RoomBooking.Application.DTOs.AddressInfo;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models.User;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;
using RoomBooking.Application.DTOs.User;
using RoomBooking.Application.Validations.Abstractions.Validators;


namespace RoomBooking.Application.Services;

public class UserService(
    IUserRepository userRepository,
    ILogger<UserService> logger,
    IUserGettingValidator userGettingValidator,
    IValidator<User> userCreationValidator,
    IValidator<UserUpdateDto> userUpdateValidator,
    IValidator<AddresInfoAddingDto>  addressInfoDtoValidator,
    IValidationToErrorConverter toErrorConverter) : IUserService
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
        
        var validationResult = userCreationValidator.Validate(instance: user);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("{@MethodName} Validation errors occured while creating a new user: {@Erros}",
                nameof(CreateUser), validationResult.Errors);

            var errors = toErrorConverter.ValidationToErrors(validationResult.Errors);
            
            return Result<Guid>.Failures(errors);
        }
       
        var userId = await userRepository.Create(user, cancellationToken);
        logger.LogInformation("{@MethodName}: User created successfully: UserId: {@UserId}", 
            nameof(CreateUser), userId);
        return userId;
    }

    public async Task<Result<ITuple>> UpdateUser(Guid id, string name, string email, string department, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@Methodname} Attempting to update user: {@UserId}",
            nameof(UpdateUser), id);
        
        var canUpdate = await userGettingValidator.IsUserExists(id, cancellationToken);

        var userUpdateDto = new UserUpdateDto(name, email, department);

        var validationResult = userUpdateValidator.Validate(userUpdateDto);
        
        if (!canUpdate || !validationResult.IsValid)
        {
            logger.LogWarning("{@MethodName}: Cannot update unexisting user: {@UserId}",
                nameof(UpdateUser), id);
            
           var errors = toErrorConverter.ValidationToErrors(validationResult.Errors);

           if (!canUpdate)
           {
               errors.Add(UserErrors.UserNotFound);
               logger.LogInformation("{@MethodName} User {@UserId} does not exist",
                   nameof(UpdateUser), id);
           }
                
            
            return Result<ITuple>.Failures(errors);
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
        
        var canDelete = await userGettingValidator.IsUserExists(id, cancellationToken);
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

        var canUpdate = await userGettingValidator.IsUserExists(id, cancellationToken);

        if (!canUpdate)
        {
            logger.LogWarning("{@MethodName}: Cannot add address to an unexisting user: {@UserId}",
                nameof(AddAddressInfo), id);
            return Result<ITuple>.Failures([UserErrors.UserNotFound]);
        }
        
        var addressInfoDto = new AddresInfoAddingDto(street, city, state, postalCode, country);
        
        var validationResult = addressInfoDtoValidator.Validate(addressInfoDto);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("{@MethodName} Validation errors occured while adding the AddressInfo: {@AddressInfoDto}",
                nameof(AddAddressInfo), addressInfoDto);
            
            var errors = toErrorConverter.ValidationToErrors(validationResult.Errors);
            
            return Result<ITuple>.Failures(errors);
        }
        
        var addedInfo = await userRepository.AddAddressInfo(
            id, street, city, state, postalCode, country, cancellationToken);
        logger.LogInformation("{@MethodName}: AddressInfo was successfully added: {@AddressInfoDto}",
            nameof(AddAddressInfo), addedInfo);

        return Result<ITuple>.Success(addedInfo);
    }
}