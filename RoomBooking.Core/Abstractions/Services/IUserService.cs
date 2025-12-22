using System.Runtime.CompilerServices;
using RoomBooking.Core.Models;
using RoomBooking.Core.Results;

namespace RoomBooking.Core.Abstractions.Services;

public interface IUserService
{
    Task<Result<List<User>>> GetAllUsers(CancellationToken cancellationToken = default);
    Task<Result<User?>> GetUserById(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateUser(User user, CancellationToken cancellationToken = default);

    Task<Result<ITuple>> UpdateUser(Guid id, string name, string email, string department, 
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> DeleteUser(Guid id, CancellationToken cancellationToken = default);
}