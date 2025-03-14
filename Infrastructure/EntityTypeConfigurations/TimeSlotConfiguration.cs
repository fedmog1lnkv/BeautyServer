using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable(TableNames.TimeSlots);

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.StaffId)
            .IsRequired();

        builder.Property(ts => ts.VenueId)
            .IsRequired();

        builder.Property(ts => ts.Weekday)
            .IsRequired();

        builder.OwnsMany(ts => ts.Intervals, intervalBuilder =>
        {
            intervalBuilder.Property(i => i.Start)
                .HasColumnName("Start")
                .IsRequired();

            intervalBuilder.Property(i => i.End)
                .HasColumnName("End")
                .IsRequired();
        });
        
        builder.HasOne(ts => ts.Venue)
            .WithMany()
            .HasForeignKey(ts => ts.VenueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}