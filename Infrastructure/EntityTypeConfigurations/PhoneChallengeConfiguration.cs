using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class PhoneChallengeConfiguration : IEntityTypeConfiguration<PhoneChallenge>
{
    public void Configure(EntityTypeBuilder<PhoneChallenge> builder)
    {
        builder.ToTable(TableNames.PhoneChallenges);

        builder.Property(p => p.Id)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.VerificationCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(p => p.ExpiredAt)
            .IsRequired();
    }
}