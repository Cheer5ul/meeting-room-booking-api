using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Models.User;
using RoomBooking.DataAccess.DbContext;
using RoomBooking.DataAccess.Entities.UserEntity;
using RoomBooking.DataAccess.Exceptions;

namespace RoomBooking.DataAccess.Repositories;

public class UserRepository(RoomBookingDbContext dbContext) : IUserRepository
{
    public async Task<List<User>> Get(CancellationToken cancellationToken = default)
    {
        var userEntites = await dbContext.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        var users = userEntites
            .Select(u => User.Create(u.Id, u.Name, u.Email, u.Department).user)
            .ToList();

        return users;
    }

    //Extra With Bookings
    // public async Task<List<User>> GetWithBookings(CancellationToken cancellationToken = default)
    // {
    //     var userEntitiesWithBookings =  await _dbContext.Users
    //         .Include(u => u.Bookings)
    //         .AsNoTracking()
    //         .OrderBy(u => u.Name)
    //         .ThenBy(u => u.Bookings.Count)
    //         .ToListAsync(cancellationToken);
    //     
    //     var userEntities = userEntitiesWithBookings
    //         .Select(u => User.Create(u.Id, u.Name, u.Email, u.Department,
    //             (UserBookingMapper.ToDomainList(u.Bookings))))
    //         .ToList();
    //     
    //     return userEntities;
    // }

    public async Task<User?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var userEntity = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        
        if (userEntity == null)
            return null;
        
        var user = User.Create(
            userEntity.Id,
            userEntity.Name, 
            userEntity.Email,
            userEntity.Department).user;
        
        return user;
    }

    public async Task<Guid> Create(User user, CancellationToken cancellationToken = default)
    {
        var userEntity = new UserEntity
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Department = user.Department
        };

        await dbContext.Users.AddAsync(userEntity, cancellationToken);

        // try
        // {
            await dbContext.SaveChangesAsync(cancellationToken);
        // }
        // catch (DbUpdateException exception)
        //     when (exception.InnerException is NpgsqlException {SqlState: PostgresErrorCodes.UniqueViolation } )
        // {
        //     throw new EmailAlreadyInUseException(user.Email, exception);
        // }
        
        return userEntity.Id;
    }
    public async Task<ITuple> Update(Guid id, string name, string email, string department, 
        CancellationToken cancellationToken = default)
    {
       await dbContext.Users.Where(u => u.Id == id)
            .ExecuteUpdateAsync(u => u
                .SetProperty(y => y.Name, name)
                .SetProperty(y => y.Email, email)
                .SetProperty(y => y.Department, department), cancellationToken);
       
        return (id, name, email, department);
    }

    public async Task<Guid> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.Where(u => u.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        
        return id;
    }
    
    public async Task<ITuple> AddAddressInfo(
        Guid id, string street, string city, string state, string postalCode, string country, 
        CancellationToken cancellationToken = default)
    {
        var userEntity = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (userEntity == null)
            throw new NotFoundException($"User with id {id} not found"); //throwing an id is not safe | need refactor

        userEntity.AddressInfo = new AddressInfoEntity
        {
            Street = street,
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = country
        };

        await dbContext.SaveChangesAsync(cancellationToken);

        
        return (street, city, state, postalCode, country);
    }
}