using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RoomBooking.Application.Services.Room;
using RoomBooking.Application.Validations.Converters;
using RoomBooking.Application.Validations.Validators.Rooms;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models.Room;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Tests.DI;

public class RoomServiceTests
{
    private const string EXISTING_ROOM_ID = "490fb600-5f7b-4e79-b6ef-40c38d80f6a4"; 
    
    // Fabric method (instead of constructor) to isolate each test
    private (IRoomService roomService, Mock<IRoomRepository> repoMock) CreateSut()
    {
        var repoMock = new Mock<IRoomRepository>();
        var loggerMock = new Mock<ILogger<RoomService>>();
        var validationSettings = Options.Create(new RoomValidationSettings()
        {
            MaximumNameLength = 50
        });

        var service = new RoomService(
            repoMock.Object,
            new RoomGettingValidator(repoMock.Object),
            new RoomCreationValidator(validationSettings),
            new RoomUpdateValidator(),
            loggerMock.Object,
            new ValidationToErrorConverter());
        
        return (service, repoMock);
    }

    #region GetRoomById
    
    [Fact]
    public async Task GetRoomById_Should_PassCancellationToken_ToRepository()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var id = Guid.NewGuid();
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        repoMock.Setup(r => r.GetById(id, token))
            .ReturnsAsync((Room?)null);

        // Act
        await sut.GetRoomById(id, token);
        
        // Assert
        repoMock.Verify(r => r.GetById(id, token), Times.Once);
    }

    [Fact]
    public async Task GetRoomById_Should_ThrowException_WhenRepositoryThrows()
    {
        //Arrange
        var (sut, repoMock) = CreateSut();
        var id = Guid.NewGuid();
        repoMock.Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Db connection lost"));
        
       // Act & Assert
       await Assert.ThrowsAsync<InvalidOperationException>(
           () => sut.GetRoomById(id, CancellationToken.None));
    }

    [Fact] //Not to interact with the repo when the input is invalid
    public async Task GetRoomById_Should_ReturnValidationError_WhenIdIsEmpty()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var emptyId = Guid.Empty;
        
        // Act
        var result = await sut.GetRoomById(emptyId, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(RoomErrors.InvalidId, result.Errors);
        repoMock.Verify(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        
    }
    
    [Fact]
    public async Task GetRoomById_Should_ReturnRoomNotFoundError_WhenGettingAnUnexistingRoom()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var id = Guid.Parse(EXISTING_ROOM_ID);
        
        repoMock.Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);
        
        // Act
        var result = await sut.GetRoomById(id, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(RoomErrors.RoomNotFound, result.Errors);
        repoMock.Verify(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), 
            Times.Once); // check if repository was used for 1 time 
    }

    [Fact]
    public async Task GetRoomById_Should_ReturnResultRoom()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var id = Guid.NewGuid();
        var roomName = "telephone booth";
        int roomCapacity = 10;
        
        var room = Room.Create(id, roomName, roomCapacity, false, false, false);
            
        repoMock.Setup(r => r.GetById(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room.room);
        
        // Act
        var result = await sut.GetRoomById(id, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsFailure);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(roomName, result.Value.Name);
        Assert.Equal(roomCapacity, result.Value.Capacity);
        
        repoMock.Verify(r => r.GetById(id, It.IsAny<CancellationToken>()),
            Times.Once); // check if repository was used for 1 time 
    }
    #endregion

    #region GetAllRooms

    [Fact]
    public async Task GetAllRooms_Should_PassCancellationToken_ToRepository()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        repoMock.Setup(r => r.Get(token))
            .ReturnsAsync([]);

        // Act
        await sut.GetAllRooms(token);
        
        // Assert
        repoMock.Verify(r => r.Get(token), Times.Once);
    }

    [Fact]
    public async Task GetAllRooms_Should_ReturnResultRooms()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var room1 = Room.Create(Guid.NewGuid(), "Room1", 10, true, true, true);
        var room2 = Room.Create(Guid.NewGuid(), "Room2", 20, true, true, false);
        var room3 = Room.Create(Guid.NewGuid(), "Room3", 30, true, false, false);

        var roomsLis = new List<Room>() { room1.room, room2.room, room3.room };
        
        repoMock.Setup(r => r.Get(It.IsAny<CancellationToken>()))
            .ReturnsAsync(roomsLis);
        
        // Act
        var result = await sut.GetAllRooms(CancellationToken.None);
        
        // Assert 
        Assert.NotNull(result);
        Assert.Equal(roomsLis, result.Value);
        Assert.False(result.IsFailure);
        repoMock.Verify(r => r.Get(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllRooms_Should_ReturnEmptyList_WhenNoRoomsExist()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();

        repoMock.Setup(r => r.Get(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await sut.GetAllRooms(CancellationToken.None);
        
        // Assert
        Assert.Empty(result.Value!);
        repoMock.Verify(r => r.Get(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllRooms_Should_ThrowException_WhenRepositoryThrows()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        repoMock.Setup(r => r.Get(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Db connection lost"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.GetAllRooms(CancellationToken.None));
    }
    #endregion
    
    #region CreateRoom

    [Fact]
    public async Task CreateRoom_Should_PassCancellationToken_ToRepository()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var room = Room.Create(Guid.NewGuid(), "Room1", 10, true, true, true);
        
        repoMock.Setup(r => r.Create(room.room, token))
            .ReturnsAsync(Guid.Empty);
        
        // Act
        await sut.CreateRoom(room.room, token);

        // Assert
        repoMock.Verify(r => r.Create(room.room, token), Times.Once);
    }

    [Fact]
    public async Task CreateRoom_Should_ThrowException_WhenRepositoryThrows()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var room = Room.Create(Guid.NewGuid(), "Room1", 10, true, true, true);
        
        repoMock.Setup(r => r.Create(room.room, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Db connection lost"));
        
        // Act && Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.CreateRoom(room.room, CancellationToken.None));
    }

    [Fact]
    public async Task CreateRoom_Should_ReturnValidationError_WhenEmptyName()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var room = Room.Create(Guid.NewGuid(), "", 10, true, true, true);
        
        // Act
        var result = await sut.CreateRoom(room.room, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(RoomErrors.NameRequired, result.Errors);
        repoMock.Verify(r => r.Create(room.room, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateRoom_Should_ReturnValidationError_WhenTooLongName()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var room = Room.Create(Guid.NewGuid(), 
            "RoomNameRoomNameRoomNameRoomNameRoomNameRoomNameRoomNameRoomNameRoomNameRoomNameRoomNameRoomNameRoomNameRoomNameRoomNameRoomName",
            10, true, true, true);
        
        // Act
        var result = await sut.CreateRoom(room.room, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(RoomErrors.NameTooLong, result.Errors);
        repoMock.Verify(r => r.Create(room.room, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateRoom_Should_ReturnValidationError_WhenCapacityIsZero()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var room = Room.Create(Guid.NewGuid(), "Room", 0, true, true, true);
        
        // Act
        var result = await sut.CreateRoom(room.room, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(RoomErrors.CapacityGreaterThanZero, result.Errors);
        repoMock.Verify(r => r.Create(room.room, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateRoom_Should_ReturnResultGuid()
    {
        // Arrange
        var (sut, repoMock) = CreateSut();
        var room = Room.Create(Guid.NewGuid(), "Room", 10, true, true, true);
        repoMock.Setup(r => r.Create(room.room, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room.room.Id);
        
        // Act
        var result = await sut.CreateRoom(room.room, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
        Assert.Equal(room.room.Id, result.Value);
        
        repoMock.Verify(r => r.Create(room.room, It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion
}