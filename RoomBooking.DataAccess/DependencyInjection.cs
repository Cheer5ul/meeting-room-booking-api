using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.DataAccess.DbContext;
using RoomBooking.DataAccess.Repositories;

namespace RoomBooking.DataAccess;

public static class DependencyInjection
{
    //Adds context DB and registers it 

    public static IServiceCollection AddPersistence(this IServiceCollection service,
        IConfiguration configuration)
    {
        var connectionstring = configuration.GetConnectionString(nameof(RoomBookingDbContext));

        if (string.IsNullOrEmpty(connectionstring))
        {
            throw new InvalidOperationException(
                "Connection string is null or empty");
        }
            
        service.AddDbContext<RoomBookingDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(nameof(RoomBookingDbContext)));
        });

        service.AddScoped<IRoomRepository, RoomRepository>();
        service.AddScoped<IUserRepository, UserRepository>();
        service.AddScoped<IBookingRepository, BookingRepository>();
        
        return service;
    }
}