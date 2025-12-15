using RoomBooking.DataAccess.DbContext;

namespace RoomBooking.DataAccess;

public class DbInitializer
{
    public static async Task InitializeAsync(RoomBookingDbContext context)
    {
        //DS says it's better to use Migrate for Production
        //await context.Database.MigrateAsync();

        await context.Database.EnsureCreatedAsync();
    }
}