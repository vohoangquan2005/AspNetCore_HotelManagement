using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagement.Data.Configuration
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            // Table
            builder.ToTable("AuditLogs");
            // Primary Key
            builder.HasKey(a => a.AuditLogId);
            // UserId
            builder.Property(a => a.UserId);
            // Action
            builder.Property(a => a.Action)
                   .IsRequired()
                   .HasMaxLength(50);
            // TableName
            builder.Property(a => a.TableName)
                   .IsRequired()
                   .HasMaxLength(100);
            // RecordId
            builder.Property(a => a.RecordId)
                   .IsRequired();
            // OldValue
            builder.Property(a => a.OldValue)
                   .HasMaxLength(4000);
            // NewValue
            builder.Property(a => a.NewValue)
                   .HasMaxLength(4000);
            // CreateAt
            builder.Property(a => a.CreateAt)
                   .HasDefaultValueSql("GETDATE()");
            // Relationship
            // User 1 - N AuditLog
            builder.HasOne(a => a.User)
                   .WithMany(u => u.AuditLogs)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
