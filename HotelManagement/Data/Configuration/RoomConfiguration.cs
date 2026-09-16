using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagement.Data.Configuration
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            // Table
            builder.ToTable("Rooms");
            // Primary
            builder.HasKey(r => r.RoomId);
            // RoomTypeId
            builder.Property(r => r.RoomTypeId)
                   .IsRequired();
            //RoomName
            builder.Property(r => r.RoomName)
                   .IsRequired()
                   .HasMaxLength(50);
            // Capacity
            builder.Property(r => r.Capacity)
                   .IsRequired();
            builder.HasIndex(r => r.RoomName)
                    .IsUnique();
            // Location
            builder.Property(r => r.Location)
                   .HasMaxLength(100);
            // PricePerNight
            builder.Property(r => r.PricePerNight) 
                   .IsRequired()
                   .HasPrecision(18, 2);
            // CreateAt
            builder.Property(r => r.CreateAt)
                   .HasDefaultValueSql("GETDATE()");
            // UpdateAt
            builder.Property(r => r.UpdateAt)
                   .HasDefaultValueSql("GETDATE()");
            // Relationship
            // Room n-1 RoomType
            builder.HasOne(r => r.RoomType)
                   .WithMany(rt => rt.Rooms)
                   .HasForeignKey(r => r.RoomTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
