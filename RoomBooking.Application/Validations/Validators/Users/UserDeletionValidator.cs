using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Core.Abstractions.Repositories;

namespace RoomBooking.Application.Validations.Validators.Users;

public class UserDeletionValidator : IUserDeletionValidator
{
    private readonly IUserRepository _userRepository;
    
    public UserDeletionValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<bool> IsUserExists(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(id, cancellationToken);

        return user != null;
    }
}