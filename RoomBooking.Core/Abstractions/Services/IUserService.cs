namespace RoomBooking.Core.Abstractions.Services;

public interface IUserService
{
    Task<List<User>> GetAllUsers(CancellationToken cancellationToken = default);
    Task<User?> GetUserById(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateUser(User user, CancellationToken cancellationToken = default);

    Task<Guid> UpdateUser(Guid id, string name, string email, string department, 
        CancellationToken cancellationToken = default);

    Task<Guid> DeleteUser(Guid id, CancellationToken cancellationToken = default);
}