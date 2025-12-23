using Microsoft.EntityFrameworkCore;
using RoomBooking.DataAccess.Configurations;
using RoomBooking.DataAccess.Entities;
using RoomBooking.DataAccess.Entities.BookingEntity;
using RoomBooking.DataAccess.Entities.RoomEntity;
using RoomBooking.DataAccess.Entities.UserEntity;

namespace RoomBooking.DataAccess.DbContext;

public class RoomBookingDbContext(DbContextOptions<RoomBookingDbContext> options)
    : Microsoft.EntityFrameworkCore.DbContext(options)
{
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
    
}