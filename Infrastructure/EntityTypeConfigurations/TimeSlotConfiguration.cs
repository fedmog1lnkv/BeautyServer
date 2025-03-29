using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        var valueComparer = new ValueComparer<List<Interval>>(
            (c1, c2) => (c1 == null && c2 == null) || c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property(ts => ts.Intervals)
            .HasConversion(
                intervals => JsonConvert.SerializeObject(intervals),
                json => DeserializeIntervals(json),
                valueComparer)
            .HasColumnType("jsonb")
            .IsRequired();
    }

    private List<Interval> DeserializeIntervals(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return [];

        var intervals = JsonConvert.DeserializeObject<List<IntervalDto>>(json);
        if (intervals is null)
            return [];

        var result = intervals
            .Select(
                interval =>
                {
                    var createResult = Interval.Create(interval.Start, interval.End);
                    return createResult.IsSuccess ? createResult.Value : null;
                })
            .Where(interval => interval is not null)
            .Cast<Interval>()
            .ToList();

        return result;
    }

    private sealed class IntervalDto
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}