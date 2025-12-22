using RoomBooking.Application.Validations.Abstractions.Users;
using RoomBooking.Core.Abstractions.Repositories;

namespace RoomBooking.Application.Validations.Validators.Users;

public class UserGettingValidator : IUserGettingValidator
{
    private readonly IUserRepository _userRepository;
    
    public UserGettingValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<bool> IsUserExists(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(id, cancellationToken);

        return user != null;
    }
}