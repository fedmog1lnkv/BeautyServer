using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class VenuePhotoConfiguration : IEntityTypeConfiguration<VenuePhoto>
{
    public void Configure(EntityTypeBuilder<VenuePhoto> builder)
    {
        builder.ToTable(TableNames.VenuePhotos);

        builder.HasKey(vp => vp.Id);
        
        builder.Property(vp => vp.Id)
            .ValueGeneratedNever();

        builder.Property(vp => vp.Order)
            .IsRequired();
        
        builder.Property(vp => vp.VenueId)
            .IsRequired();

        builder.Property(vp => vp.PhotoUrl)
            .IsRequired()
            .HasMaxLength(2048);
    }
}