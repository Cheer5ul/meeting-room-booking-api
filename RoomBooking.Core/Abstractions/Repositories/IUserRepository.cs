using System.Runtime.CompilerServices;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.User;

namespace RoomBooking.Core.Abstractions.Repositories;

public interface IUserRepository
{
    Task<List<User>> Get(CancellationToken cancellationToken = default);
    Task<User?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> Create(User user, CancellationToken cancellationToken = default);

    Task<ITuple> Update(Guid id, string name, string email, string department, 
        CancellationToken cancellationToken = default);

    Task<Guid> Delete(Guid id, CancellationToken cancellationToken = default);

    Task<ITuple> AddAddressInfo(
        Guid id, string street, string city, string state, string postalCode, string country,
        CancellationToken cancellationToken = default);
}