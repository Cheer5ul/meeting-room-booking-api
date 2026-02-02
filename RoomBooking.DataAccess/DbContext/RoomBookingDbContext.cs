using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoomBooking.DataAccess.Configurations;
using RoomBooking.DataAccess.Entities;
using RoomBooking.DataAccess.Entities.BookingEntity;
using RoomBooking.DataAccess.Entities.RoomEntity;
using RoomBooking.DataAccess.Entities.UserEntity;

namespace RoomBooking.DataAccess.DbContext;

public class RoomBookingDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public RoomBookingDbContext(DbContextOptions<RoomBookingDbContext> options)
        : base(options)
    {
        
    }
    public RoomBookingDbContext()
    {
    }
    public DbSet<RoomEntity> Rooms { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<BookingEntity> Bookings { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoomConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        
        base.OnModelCreating(modelBuilder);         
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (!optionsBuilder.IsConfigured)
    //     {
    //         var connectionString =
    //             ;
    // 
    //         if (string.IsNullOrEmpty(connectionString))
    //         {
    //             throw new InvalidOperationException(
    //                 "Connection string is null or empty");
    //         }
    //         
    //         optionsBuilder.UseNpgsql(connectionString);
    //         optionsBuilder.EnableSensitiveDataLogging();
    //         optionsBuilder.EnableDetailedErrors();
    //     }
    //     
    // }
}