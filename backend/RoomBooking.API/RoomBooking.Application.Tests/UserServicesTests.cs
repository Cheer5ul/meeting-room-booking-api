using FluentValidation;
using Microsoft.Extensions.Logging;
using RoomBooking.Application.DTOs.AddressInfo;
using RoomBooking.Application.DTOs.User;
using RoomBooking.Application.Interfaces.Auth.Hashers;
using RoomBooking.Application.Interfaces.Auth.Providers;
using RoomBooking.Application.Services.User.Services;
using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Application.Validations.Abstractions.Validators;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Models.User;
using RoomBooking.Core.Results.Errors;
using Xunit;

namespace RoomBooking.Application.Tests;

public class UserServicesTests()
{
    private static readonly IUserRepository userRepository;
    private static readonly ILogger<UserService> logger;
    private static readonly IUserGettingValidator userGettingValidator;
    private static readonly IUserEmailValidator userEmailValidator;
    private static readonly IValidator<User> userCreationValidator;
    private static readonly IValidator<UserUpdateDto> userUpdateValidator;
    private static readonly IValidator<AddresInfoAddingDto>  addressInfoDtoValidator;
    private static readonly IValidationToErrorConverter toErrorConverter;
    private static readonly IUserPasswordHasher userPasswordHasher;
    private static readonly IJwtProvider jwtProvider;
    
    
    [Fact]
    public async Task CreateUser_Should_ReturnError_WhenCreatingWithAnAlreadyExistingEmail()
    {

        var userService = new UserService(
            userRepository, logger, userGettingValidator, userEmailValidator, userCreationValidator,
            userUpdateValidator, addressInfoDtoValidator, toErrorConverter, userPasswordHasher, jwtProvider);
    
        // Arrange

        
        
        var name = "test_name";
        var email = "test_existing_email@email.com";
        var department = "t1";
        var password = "test_password";
        
        // Act
        var result = await userService.CreateUser(name, email, password, department, default);
        
        // Assert
        Assert.Equal(UserErrors.EmailAlreadyUsed, result.Errors.First());
            
    }
}