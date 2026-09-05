using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Core.Abstractions.Repositories;

namespace RoomBooking.Application.Validations.Validators.Users;

public class UserEmailValidator : IUserEmailValidator
{
    private readonly IUserRepository _userRepository;

    public UserEmailValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> IsEmailAlreadyUsed(string email, CancellationToken cancellationToken)
    {
        var usersToCheck = await _userRepository.Get(cancellationToken);

        bool isEmailUsed = usersToCheck.Any(u => u.Email == email);
        
        return isEmailUsed;
    }
}