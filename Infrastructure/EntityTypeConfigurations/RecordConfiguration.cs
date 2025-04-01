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

        builder.Property(r => r.Status)
            .HasConversion(
                v => v.ToString(),
                v => (RecordStatus)Enum.Parse(typeof(RecordStatus), v, true))
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(s => s.Comment)
            .HasConversion(
                comment => comment == null ? null : comment.Value,
                value => value == null ? null : RecordComment.Create(value).Value)
            .HasMaxLength(RecordComment.MaxLength);
        
        builder.Property(s => s.CreatedOnUtc)
            .IsRequired();

        builder.Property(s => s.ModifiedOnUtc)
            .IsRequired(false);
    }
}