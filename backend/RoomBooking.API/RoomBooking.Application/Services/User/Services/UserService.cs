using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Npgsql;
using RoomBooking.Application.DTOs.AddressInfo;
using RoomBooking.Application.DTOs.User;
using RoomBooking.Application.Interfaces.Auth.Hashers;
using RoomBooking.Application.Interfaces.Auth.Providers;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Application.Validations.Abstractions.Validators;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models.User;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;
using RoomBooking.DataAccess.Exceptions;

namespace RoomBooking.Application.Services.User.Services;

public class UserService(
    IUserRepository userRepository,
    ILogger<UserService> logger,
    IUserGettingValidator userGettingValidator,
    IUserEmailValidator userEmailValidator,
    IValidator<Core.Models.User.User> userCreationValidator,
    IValidator<UserUpdateDto> userUpdateValidator,
    IValidator<AddresInfoAddingDto>  addressInfoDtoValidator,
    IValidationToErrorConverter toErrorConverter,
    IUserPasswordHasher userPasswordHasher,
    IJwtProvider jwtProvider) : IUserService
{
    public async Task<Result<List<Core.Models.User.User>>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        return await userRepository.Get(cancellationToken);
    }

    public async Task<Result<Core.Models.User.User?>> GetUserById(Guid id, CancellationToken cancellationToken = default)
    {
        
        logger.LogInformation("{@MethodName}: Getting user by id {@Id}",
            nameof(GetUserById), id);
        
        // fast check | maybe should be even in the controller | NEEDS REFACTOR
        if (id == Guid.Empty)
        {
            logger.LogInformation("{@MethodName}: Getting user with an empty id {@UserId}",
                nameof(GetUserById), id);
            return Result<Core.Models.User.User?>.Failures([UserErrors.EmptyId]);
        }
        
        logger.LogInformation("{@MethodName}: Getting user by id: {@UserId}", nameof(GetUserById), id);
        var user = await userRepository.GetById(id, cancellationToken);
        
        if (user == null)
        {
            logger.LogInformation("User not found: {@UserId}", id);
            return Result<Core.Models.User.User?>.Failures([UserErrors.UserNotFound]);
        }
        return user;
    }

    public async Task<Result<Guid>> CreateUser(
        string name, string email, string department, string password,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Creating new user. Name: {@UserName}, Email: {@UserEmail}",
            nameof(CreateUser), name, email);

        var hashedPassword = userPasswordHasher.Generate(password);
        
        var user = Core.Models.User.User.Create(name, email, department, hashedPassword);
        
        if(user.IsFailure)
            return Result<Guid>.Failures(user.Errors);
        
        var validationResult = await userCreationValidator.ValidateAsync(user.Value!, cancellationToken);

        bool isEmailUsed = await userEmailValidator.IsEmailAlreadyUsed(user.Value!.Email, cancellationToken);

        if (!validationResult.IsValid || isEmailUsed)
        {
            logger.LogInformation("{@MethodName} Validation errors occured while creating a new user: {@Errors}",
                nameof(CreateUser), validationResult.Errors);

            var errors = toErrorConverter.ValidationToErrors(validationResult.Errors);
            if(isEmailUsed)
                errors.Add(UserErrors.EmailAlreadyUsed);
            
            return Result<Guid>.Failures(errors);
        }

        try
        {
            var userId = await userRepository.Create(user.Value, cancellationToken);
            logger.LogInformation("{@MethodName}: User created successfully: UserId: {@UserId}",
                nameof(CreateUser), userId);
            return userId;
        }
        catch (Exception exception)
            when (exception.InnerException is NpgsqlException {SqlState: PostgresErrorCodes.UniqueViolation })
        {
            logger.LogWarning("{@MethodName} race condition occured while creating users with the same email {@Exception}",
                nameof(CreateUser), exception);
            return Result<Guid>.Failures([UserErrors.EmailAlreadyUsed]);
        }
    }

    public async Task<Result<string>> Login(string email, string password, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName}: Attempting to login user with email {@UserEmail}",
            nameof(Login), email);
        Core.Models.User.User? user = await userRepository.GetByEmail(email, cancellationToken);

        if (user == null)
        {
            logger.LogInformation("User with email {@Email} was not found", email);
            return Result<string>.Failures([UserErrors.UserNotFound]);
        }

        var result = userPasswordHasher.Verify(password, user.PasswordHash);

        if (result == false)
        {
            logger.LogInformation("Invalid login attempt with email {@Email}, Incorrect password", email);
            return Result<string>.Failures([UserErrors.IncorrectPassword]);
        }
        
        //creating a jwt token
        string token = jwtProvider.GenerateToken(user);

        return token;
    }

    public async Task<Result<ITuple>> UpdateUser(Guid id, string name, string email, string department, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{@MethodName} Attempting to update user: {@UserId}",
            nameof(UpdateUser), id);
        
        var canUpdate = await userGettingValidator.IsUserExists(id, cancellationToken);

        var userUpdateDto = new UserUpdateDto(name, email, department);
        
        var validationResult = await userUpdateValidator.ValidateAsync(userUpdateDto, cancellationToken);
        
        if (!canUpdate || !validationResult.IsValid)
        {
            logger.LogWarning("{@MethodName}: Cannot update unexisting user: {@UserId}",
                nameof(UpdateUser), id);
            
           var errors = toErrorConverter.ValidationToErrors(validationResult.Errors);

           if (!canUpdate)
           {
               logger.LogInformation("{@MethodName} User {@UserId} does not exist",
                   nameof(UpdateUser), id);
               errors.Add(UserErrors.UserNotFound);
           }
            
            return Result<ITuple>.Failures(errors);
        }
        
        var affectedRows = await userRepository.Update(
            id,
            userUpdateDto.Name,
            userUpdateDto.Email,
            userUpdateDto.Department,
            cancellationToken);

        logger.LogInformation("{@MethodName}: Updating user {@UserId}",
            nameof(UpdateUser), id);

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
        Guid id, AddressInfo addressInfo,
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
        
        var addressInfoDto = new AddresInfoAddingDto(
            addressInfo.Street, 
            addressInfo.City,
            addressInfo.State, 
            addressInfo.PostalCode,
            addressInfo.Country);
        
        var validationResult = await addressInfoDtoValidator.ValidateAsync(addressInfoDto, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("{@MethodName} Validation errors occured while adding the AddressInfo: {@AddressInfoDto}",
                nameof(AddAddressInfo), addressInfoDto);
            
            var errors = toErrorConverter.ValidationToErrors(validationResult.Errors);
            
            return Result<ITuple>.Failures(errors);
        }

        //NEEDS REFACTOR
        try
        {
            var addedInfo = await userRepository.AddAddressInfo(
                id,
                addressInfo.Street,
                addressInfo.City,
                addressInfo.State,
                addressInfo.PostalCode,
                addressInfo.Country, cancellationToken);
            
            logger.LogInformation("{@MethodName}: AddressInfo was successfully added: {@AddressInfoDto}",
                nameof(AddAddressInfo), addedInfo);

            return Result<ITuple>.Success(addedInfo);
        }
        catch (Exception exception)
            when (exception is NotFoundException)
        {
            throw new NotFoundException($"User with id {id} not found");
        }
    }

    public async Task<int> DeleteAllUsers(CancellationToken cancellationToken = default)
    {
        return await userRepository.DeleteAll(cancellationToken);
    }
}