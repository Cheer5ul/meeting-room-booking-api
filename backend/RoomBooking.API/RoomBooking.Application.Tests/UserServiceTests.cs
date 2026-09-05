using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RoomBooking.Application.Interfaces.Auth.Hashers;
using RoomBooking.Application.Services.User.Services;
using RoomBooking.Application.Validations.Converters;
using RoomBooking.Application.Validations.Validators.Users;
using RoomBooking.Application.Validations.Validators.Users.AddressInfo;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models.User;
using RoomBooking.Core.Results.Errors;
using RoomBooking.Infrastructure.Hashers;
using RoomBooking.Infrastructure.Providers;
using Xunit;

namespace RoomBooking.Application.Tests;

public class UserServiceTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IUserService _userService;
    private readonly IUserPasswordHasher _hasher;
    private readonly ILogger<UserService> _loggerMock;

    public UserServiceTests()
    {
        IOptions<JwtOptions> options = Options.Create(new JwtOptions()
        {
            SecretKey = "mySecretKeymySecretKeymySecretKey",
            ExpiresHours = 1
        });
        
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _loggerMock = Substitute.For<ILogger<UserService>>();
        _userService = new UserService(
            _userRepositoryMock,
            _loggerMock,
            new UserGettingValidator(_userRepositoryMock),
            new UserEmailValidator(_userRepositoryMock),
            new UserCreationValidator(),
            new UserUpdateValidator(),
            new AddressInfoAddingDtoValidator(),
            new ValidationToErrorConverter(),
            new UserPasswordHasher(),
            new JwtProvider(options)
         );
        
        _hasher = new UserPasswordHasher();
    }

    // [Fact]
    // public async Task GetUserById_Should_UseLogger()
    // {
    //     // Arrange 
    //     var id = Guid.NewGuid();
    //     
    //     _userRepositoryMock.GetById(id, CancellationToken.None).Returns((User?)null);
    //     // Act
    //     await _userService.GetUserById(id, CancellationToken.None);
    //     
    //     // Assert
    //     _loggerMock.Received(1).Log(
    //         LogLevel.Information,
    //         Arg.Any<Exception>(),
    //         Arg.Any<string>(),
    //         Arg.Is<object>(x => x.ToString()!.Contains("Getting user by id")));
    // }
    
    [Fact]
    public async Task GetUserById_Should_PassCancellationToken_ToRepository()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        
        _userRepositoryMock.GetById(id, token).Returns((User?)null);
        
        // Act
        await _userService.GetUserById(id, token);
        
        // Assert
        await _userRepositoryMock.Received(1).GetById(id, token);
    }

    [Fact]
    public async Task GetUserById_Should_ThrowException_WhenRepositoryThrows()
    {
        // Arrange 
        var id = Guid.NewGuid();
         
        _userRepositoryMock.GetById(id, CancellationToken.None).ThrowsAsync(new InvalidOperationException());
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.GetUserById(id, CancellationToken.None));
    }
    
    [Fact]
    public async Task GetUserById_Should_ReturnError_WhenUsingUnexistingId()
    {
        // Arrange 
        var id = Guid.NewGuid();
        _userRepositoryMock.GetById(id).Returns((User?)null);
        
        // Act 
        var result = await _userService.GetUserById(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(UserErrors.UserNotFound, result.Errors);
        await _userRepositoryMock.Received(1).GetById(id);
    }
    
    [Fact]
    public async Task GetUserById_Should_ReturnError_WhenIdIsEmpty()
    {
        // Arrange
        var id = Guid.Empty;
        
        // Act
        var result = await _userService.GetUserById(id, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(UserErrors.EmptyId, result.Errors);
        await _userRepositoryMock.Received(0).GetById(id, CancellationToken.None);
    }

    [Fact]
    public async Task GetUserById_Should_ReturnResult()
    {
        // Arrange 
        var user = User.Create("User1", "user1@test.com", "a1", _hasher.Generate("password"));
        
        _userRepositoryMock.GetById(user.Value!.Id, CancellationToken.None)
            .Returns(user.Value);

        // Act
        var result = await _userService.GetUserById(user.Value.Id, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsFailure);
        Assert.NotNull(result.Value);
        Assert.Equal(user.Value.Id, result.Value.Id);
        Assert.Equal(result.Value.Email, user.Value.Email);
        Assert.Equal(result.Value.Name, user.Value.Name);

        await _userRepositoryMock.Received(1).GetById(user.Value.Id, CancellationToken.None);
    }
}