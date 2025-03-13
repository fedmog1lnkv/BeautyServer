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
            .HasMaxLength(VenueName.MaxLength)
            .IsRequired(false);
        
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
        

    }
}