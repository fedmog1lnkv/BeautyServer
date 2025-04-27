using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class RecordStatusLogConfiguration : IEntityTypeConfiguration<RecordStatusLog>
{
    public void Configure(EntityTypeBuilder<RecordStatusLog> builder)
    {
        builder.ToTable(TableNames.RecordStatusLog);

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.RecordId)
            .IsRequired();

        builder.Property(r => r.StatusChange)
            .HasConversion(
                v => v.ToString(),
                v => (RecordStatusChange)Enum.Parse(typeof(RecordStatusChange), v, true))
            .HasColumnType("varchar(20)")
            .IsRequired();

        builder.Property(r => r.Description)
            .HasConversion(
                description => description.Value,
                value => RecordStatusLogDescription.Create(value).Value)
            .HasMaxLength(RecordStatusLogDescription.MaxLength)
            .IsRequired();

        builder.Property(r => r.Timestamp)
            .IsRequired();
    }
}