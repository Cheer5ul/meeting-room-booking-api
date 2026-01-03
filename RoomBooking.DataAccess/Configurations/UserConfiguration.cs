using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Win32.SafeHandles;
using RoomBooking.DataAccess.Entities;
using RoomBooking.DataAccess.Entities.UserEntity;

namespace RoomBooking.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(x => x.Department)
            .IsRequired()
            .HasMaxLength(50);
        
        builder
            .HasMany(x => x.Bookings)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId) //implicitly showing foreign key
            .OnDelete(DeleteBehavior.Cascade); // Cascade Delete behavior

        builder.OwnsOne(x => x.AddressInfo, address =>
        {
            address.Property(x => x.Street).HasMaxLength(100);
            address.Property(x => x.City).HasMaxLength(100);
            address.Property(x => x.State).HasMaxLength(100);
            address.Property(x => x.PostalCode).HasMaxLength(20);
            address.Property(x => x.Country).HasMaxLength(100);
        });
        
        //Using Unique Index to avoid race conditions 
        builder.HasIndex(u => u.Email).IsUnique();
    }
}