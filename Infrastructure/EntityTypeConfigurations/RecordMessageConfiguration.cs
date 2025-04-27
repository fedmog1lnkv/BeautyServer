using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class RecordMessageConfiguration : IEntityTypeConfiguration<RecordMessage>
{
    public void Configure(EntityTypeBuilder<RecordMessage> builder)
    {
        builder.ToTable(TableNames.RecordMessages);

        builder.HasKey(rm => rm.Id);
        builder.Property(rm => rm.Id)
            .ValueGeneratedNever();

        builder.Property(rm => rm.RecordId)
            .IsRequired();

        builder.Property(rm => rm.SenderId)
            .IsRequired();

        builder.Property(rm => rm.IsRead)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(rm => rm.ReadAt)
            .IsRequired(false);
        
        builder.Property(rm => rm.CreatedAt)
            .IsRequired();

        builder.Property(rm => rm.Message)
            .HasConversion(
                v => v.Value,
                v => RecordMessageContent.Create(v).Value)
            .HasMaxLength(RecordMessageContent.MaxLength)
            .IsRequired();
    }
}