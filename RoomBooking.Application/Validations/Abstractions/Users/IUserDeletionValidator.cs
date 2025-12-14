namespace RoomBooking.Application.Validations.Abstractions.Users;

public interface IUserDeletionValidator
{
    Task<bool> IsUserExists(
        Guid id,
        CancellationToken cancellationToken);
}