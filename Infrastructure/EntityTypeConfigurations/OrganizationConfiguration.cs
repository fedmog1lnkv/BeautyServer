using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable(TableNames.Organizations);

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .HasConversion(
                name => name.Value,
                value => OrganizationName.Create(value).Value)
            .HasMaxLength(OrganizationName.MaxLength)
            .IsRequired();

        builder.Property(o => o.Description)
            .HasConversion(
                description => description == null ? null : description.Value,
                value => value == null ? null : OrganizationDescription.Create(value).Value)
            .HasMaxLength(OrganizationDescription.MaxLength)
            .IsRequired(false);

        builder.Property(o => o.Subscription)
            .HasConversion(
                v => v.ToString(),
                v => (OrganizationSubscription)Enum.Parse(typeof(OrganizationSubscription), v, true)
            )
            .HasColumnType("varchar(10)")
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
    }
}