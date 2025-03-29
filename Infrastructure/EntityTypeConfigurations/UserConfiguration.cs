using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableNames.Users);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(u => u.Name)
            .HasConversion(
                u => u.Value,
                u => UserName.Create(u).Value)
            .HasMaxLength(UserName.MaxLength)
            .IsRequired();

        builder.Property(u => u.PhoneNumber)
            .HasConversion(
                u => u.Value,
                u => UserPhoneNumber.Create(u).Value)
            .HasMaxLength(15)
            .IsRequired();
        
        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique();
    }
}