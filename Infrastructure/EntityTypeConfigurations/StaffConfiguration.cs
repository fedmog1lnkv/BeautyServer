using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable(TableNames.Staffs);

        builder.HasKey(s => s.Id);

        builder.Property(s => s.OrganizationId)
            .IsRequired();

        builder.Property(s => s.Name)
            .HasConversion(
                name => name.Value,
                value => StaffName.Create(value).Value)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.PhoneNumber)
            .HasConversion(
                phone => phone.Value,
                value => StaffPhoneNumber.Create(value).Value)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(s => s.Role)
            .IsRequired();

        builder.HasMany(s => s.TimeSlots)
            .WithOne()
            .HasForeignKey(ts => ts.StaffId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(s => s.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}