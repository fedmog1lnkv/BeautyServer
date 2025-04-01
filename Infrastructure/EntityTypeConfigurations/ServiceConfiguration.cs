using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable(TableNames.Services);

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasConversion(
                name => name.Value,
                value => ServiceName.Create(value).Value)
            .HasMaxLength(ServiceName.MaxLength)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasConversion(
                description => description == null ? null : description.Value,
                value => value == null ? null : ServiceDescription.Create(value).Value)
            .HasMaxLength(ServiceDescription.MaxLength)
            .IsRequired(false);
        
        builder.Property(s => s.Duration)
            .IsRequired(false);
        
        builder.Property(s => s.Price)
            .HasConversion(
                price => price == null ? (double?)null : price.Value,
                value => value == null ? null : ServicePrice.Create((double)value).Value)
            .IsRequired(false);
        
        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(s => s.OrganizationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(s => s.CreatedOnUtc)
            .IsRequired();

        builder.Property(s => s.ModifiedOnUtc)
            .IsRequired(false);
    }
}