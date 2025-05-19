using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class RecordConfiguration : IEntityTypeConfiguration<Record>
{
    public void Configure(EntityTypeBuilder<Record> builder)
    {
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Staff)
            .WithMany()
            .HasForeignKey(r => r.StaffId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Organization)
            .WithMany()
            .HasForeignKey(r => r.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Venue)
            .WithMany()
            .HasForeignKey(r => r.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Service)
            .WithMany()
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Coupon)
            .WithMany()
            .HasForeignKey(r => r.CouponId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(r => r.OriginalAmount)
            .HasConversion(
                v => v == null ? (decimal?)null : v.Value,     
                v => v == null ? null : RecordAmount.Create(v.Value).Value 
            )
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        
        builder.Property(r => r.DiscountedAmount)
            .HasConversion(
                v => v == null ? (decimal?)null : v.Value,     
                v => v == null ? null : RecordAmount.Create(v.Value).Value 
            )
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(r => r.Status)
            .HasConversion(
                v => v.ToString(),
                v => (RecordStatus)Enum.Parse(typeof(RecordStatus), v, true))
            .HasColumnType("varchar(10)")
            .IsRequired();
        
        builder.OwnsOne(
            r => r.Review,
            theme =>
            {
                theme.Property(rr => rr.Rating)
                    .HasColumnName("Rating")
                    .IsRequired();

                theme.Property(rr => rr.Comment)
                    .HasColumnName("Comment")
                    .IsRequired(false);
            });
        
        builder.HasMany(r => r.StatusLogs)
            .WithOne()
            .HasForeignKey(sl => sl.RecordId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(r => r.Messages)
            .WithOne()
            .HasForeignKey(m => m.RecordId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(s => s.CreatedOnUtc)
            .IsRequired();

        builder.Property(s => s.ModifiedOnUtc)
            .IsRequired(false);
    }
}