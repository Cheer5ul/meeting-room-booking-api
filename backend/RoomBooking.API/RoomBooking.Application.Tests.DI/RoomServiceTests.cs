using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RoomBooking.Application.Services.Room;
using RoomBooking.Application.Validations.Converters;
using RoomBooking.Application.Validations.Validators.Rooms;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models.Room;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;
using Xunit;
using Assert = NUnit.Framework.Assert;

namespace RoomBooking.Application.Tests.DI;

public class RoomServiceTests
{
    private readonly IRoomService _roomService;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    public RoomServiceTests()
    {
        //mocking
        _roomRepositoryMock = new Mock<IRoomRepository>();
        var loggerMock = new Mock<ILogger<RoomService>>();
        
        var validationSettings = Options.Create(new RoomValidationSettings()
        {
            MaximumNameLength = 50
        });
        
        // _roomService = serviceProvider.GetRequiredService<IRoomService>();
        var roomService = new RoomService(_roomRepositoryMock.Object,
            new RoomGettingValidator(_roomRepositoryMock.Object),
            new RoomCreationValidator(validationSettings),
            new RoomUpdateValidator(),
            loggerMock.Object,
            new ValidationToErrorConverter());
        
        _roomService = roomService;
    }

    [Xunit.Theory]
    [InlineData("490fb600-5f7b-4e79-b6ef-40c38d80f6a4")]
    public async Task GetRoomById_Should_ReturnRoomNotFoundError_WhenGettingAnUnexistingRoom(Guid id)
    {
        // Arrange
        _roomRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>()))
            .ReturnsAsync((Room?)null);
        // Act
        var result = await _roomService.GetRoomById(id, CancellationToken.None);
        // Assert
        bool isNotFoundError = result.Errors[0] == RoomErrors.RoomNotFound; 
        Assert.That(isNotFoundError, Is.True);
        
    }

    [Xunit.Theory]
    [InlineData("490fb600-5f7b-4e79-b6ef-40c38d80f6a4")]
    public async Task GetRoomById_Should_ReturnResultRoom(Guid id)
    {
        // Arrange
        string roomName = "telephone booth";
        int roomCapacity = 10;
        bool hasProjector = false;
        bool hasTv = false;
        bool hasWhiteBoard = false;
        var room = Room.Create(id, roomName, roomCapacity, hasProjector, hasTv, hasWhiteBoard);

        //NEEDS FIXES
        _roomRepositoryMock.Setup(r => r.GetById(id))
            .ReturnsAsync(Result.Success());
        
        // Act
        var result = await _roomService.GetRoomById(id, CancellationToken.None);
        
        // Assert
        bool isAnyError = result.Errors.Any();
        Assert.That(isAnyError, Is.False);
    }
    
    
}