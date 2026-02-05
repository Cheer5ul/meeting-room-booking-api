using System.Runtime.CompilerServices;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.User;
using RoomBooking.Core.Results;

namespace RoomBooking.Core.Abstractions.Services;

public interface IUserService
{
    Task<Result<List<User>>> GetAllUsers(CancellationToken cancellationToken = default);
    Task<Result<User?>> GetUserById(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateUser(string name, string email, string department, string password,
        CancellationToken cancellationToken = default);

    Task<Result<string>> Login(string email, string password,
        CancellationToken cancellationToken = default);

    Task<Result<ITuple>> UpdateUser(Guid id, string name, string email, string department, 
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> DeleteUser(Guid id, CancellationToken cancellationToken = default);

    Task<Result<ITuple>> AddAddressInfo(
        Guid id, AddressInfo addressInfo,
        CancellationToken cancellationToken = default);

    Task<int> DeleteAllUsers(CancellationToken cancellationToken = default);
}