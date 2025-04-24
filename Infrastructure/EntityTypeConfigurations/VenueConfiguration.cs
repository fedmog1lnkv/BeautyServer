using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable(TableNames.Venues);

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .HasConversion(
                name => name.Value,
                value => VenueName.Create(value).Value)
            .HasMaxLength(VenueName.MaxLength)
            .IsRequired();

        builder.Property(v => v.Description)
            .HasConversion(
                description => description == null ? null : description.Value,
                value => value == null ? null : VenueDescription.Create(value).Value)
            .HasMaxLength(VenueDescription.MaxLength)
            .IsRequired(false);

        builder.Property(v => v.Address)
            .HasConversion(
                address => address.Value,
                value => VenueAddress.Create(value).Value)
            .HasMaxLength(VenueAddress.MaxLength)
            .IsRequired();

        builder.OwnsOne(
            o => o.Location,
            theme =>
            {
                theme.Property(l => l.Latitude)
                    .HasColumnName("Latitude")
                    .IsRequired();

                theme.Property(l => l.Longitude)
                    .HasColumnName("Longitude")
                    .IsRequired();
            });

        builder.Property(v => v.TimeZone)
            .HasConversion(
                tz => tz.Id,
                id => TimeZoneInfo.FindSystemTimeZoneById(id))
            .HasColumnName("TimeZoneId")
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(
            o => o.Theme,
            theme =>
            {
                theme.Property(t => t.Color)
                    .HasColumnName("Color")
                    .HasMaxLength(7)
                    .IsRequired();

                theme.Property(t => t.Photo)
                    .HasColumnName("Photo")
                    .HasMaxLength(2048)
                    .IsRequired(false);
            });

        builder.Property(s => s.CreatedOnUtc)
            .IsRequired();

        builder.Property(s => s.ModifiedOnUtc)
            .IsRequired(false);

        builder.HasMany(v => v.Photos)
            .WithOne()
            .HasForeignKey(v => v.VenueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}