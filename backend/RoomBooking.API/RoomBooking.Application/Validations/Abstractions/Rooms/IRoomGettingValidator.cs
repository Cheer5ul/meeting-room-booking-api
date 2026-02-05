namespace RoomBooking.Application.Validations.Abstractions.Rooms;

public interface IRoomGettingValidator
{
    Task<bool> IsExisting(
        Guid id, 
        CancellationToken cancellationToken = default);
}