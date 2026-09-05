using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomBooking.DataAccess.Entities;
using RoomBooking.DataAccess.Entities.RoomEntity;

namespace RoomBooking.DataAccess.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<RoomEntity>
{
    public void Configure(EntityTypeBuilder<RoomEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder
            .HasMany(x => x.Bookings)
            .WithOne(x => x.Room);
        
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.Capacity)
            .IsRequired();
        
        builder.Property(x => x.HasProjector)
            .IsRequired();
        
        builder.Property(x => x.HasTv)
            .IsRequired();
        
        builder.Property(x => x.HasWhiteBoard)
            .IsRequired();
    }
}