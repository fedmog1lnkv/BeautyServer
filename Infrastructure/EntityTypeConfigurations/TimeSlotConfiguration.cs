using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Infrastructure.EntityTypeConfigurations;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable(TableNames.TimeSlots);

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Id)
            .ValueGeneratedNever();

        builder.Property(ts => ts.StaffId)
            .IsRequired();

        builder.Property(ts => ts.VenueId)
            .IsRequired();

        builder.Property(ts => ts.Date)
            .IsRequired();

        builder.Property(ts => ts.Intervals)
            .HasConversion(
                intervals => JsonConvert.SerializeObject(intervals),
                json => DeserializeIntervals(json))
            .HasColumnType("jsonb")
            .IsRequired();
    }

    private List<Interval> DeserializeIntervals(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return new List<Interval>();

        var intervals = JsonConvert.DeserializeObject<List<IntervalDto>>(json);

        var result = intervals?
            .Select(interval =>
            {
                var createResult = Interval.Create(interval.Start, interval.End);
                return createResult.IsSuccess ? createResult.Value : null;  // Возвращаем только валидные интервалы
            })
            .Where(interval => interval != null)
            .ToList();

        return result ?? new List<Interval>();
    }
    
    private class IntervalDto
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}