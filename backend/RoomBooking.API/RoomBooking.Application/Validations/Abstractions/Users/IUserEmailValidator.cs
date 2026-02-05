namespace RoomBooking.Application.Validations.Abstractions.Users;

public interface IUserEmailValidator
{
    Task<bool> IsEmailAlreadyUsed(string email, CancellationToken cancellationToken);
}