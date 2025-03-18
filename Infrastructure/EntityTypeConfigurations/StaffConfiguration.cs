using Domain.Entities;
using Domain.Enums;
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
            .HasConversion(
                v => v.ToString(),
                v => (StaffRole)Enum.Parse(typeof(StaffRole), v, true)
            )
            .HasColumnType("varchar(10)")
            .IsRequired(); 

        builder.HasMany(s => s.TimeSlots)
            .WithOne()
            .HasForeignKey(ts => ts.StaffId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(s => s.CreatedOnUtc)
            .IsRequired();

        builder.Property(s => s.ModifiedOnUtc)
            .IsRequired(false);
        
        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(s => s.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}