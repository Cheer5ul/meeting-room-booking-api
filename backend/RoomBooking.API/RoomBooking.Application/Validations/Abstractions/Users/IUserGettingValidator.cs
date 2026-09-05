namespace RoomBooking.Application.Validations.Abstractions.Users;

public interface IUserGettingValidator
{
    Task<bool> IsUserExists(
        Guid id,
        CancellationToken cancellationToken);
}