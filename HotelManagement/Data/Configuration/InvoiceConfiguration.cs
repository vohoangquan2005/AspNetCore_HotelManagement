using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagement.Data.Configuration
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            // Table
            builder.ToTable("Invoices");
            // Primary Key
            builder.HasKey(i => i.InvoiceId);
            // BookingId
            builder.Property(i => i.BookingId)
                   .IsRequired();
            // SubTotal
            builder.Property(i => i.SubTotal)
                   .IsRequired()
                   .HasPrecision(18, 2);
            // TaxAmount
            builder.Property(i => i.TaxAmount)
                   .IsRequired()
                   .HasPrecision(18, 2);
            // DiscountAmount
            builder.Property(i => i.DiscountAmount)
                   .IsRequired()
                   .HasPrecision(18, 2);
            // AmountDue
            builder.Property(i => i.AmountDue)
                   .IsRequired()
                   .HasPrecision(18, 2);
            // Status
            builder.Property(i => i.Status)
                   .IsRequired();
            // CreatedByUserId
            builder.Property(i => i.CreatedByUserId);
            // CreateAt
            builder.Property(i => i.CreateAt)
                   .HasDefaultValueSql("GETDATE()");
            // Relationships
            // Booking 1 - N Invoice
            builder.HasOne(i => i.Booking)
                   .WithMany(b => b.Invoices)
                   .HasForeignKey(i => i.BookingId)
                   .OnDelete(DeleteBehavior.Restrict);
            // User 1 - N Invoice
            builder.HasOne(i => i.CreatedByUser)
                   .WithMany(u => u.CreatedInvoices)
                   .HasForeignKey(i => i.CreatedByUserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
