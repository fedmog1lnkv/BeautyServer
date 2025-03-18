using Domain.Errors;
using Domain.Primitives;
using Domain.Repositories.Venues;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class TimeSlot : Entity
{
    private TimeSlot(
        Guid id,
        Guid staffId,
        Guid venueId,
        DateOnly date,
        List<Interval> intervals) : base(id)
    {
        StaffId = staffId;
        VenueId = venueId;
        Date = date;
        Intervals = intervals;
    }
    
#pragma warning disable CS8618
    private TimeSlot() { }
#pragma warning restore CS8618

    public Guid StaffId { get; private set; }
    public Guid VenueId { get; private set; }
    public DateOnly Date { get; private set; }
    public List<Interval> Intervals { get; private set; }

    public static async Task<Result<TimeSlot>> CreateAsync(
        Guid id,
        Guid staffId,
        Guid venueId,
        DateOnly date,
        IVenueReadOnlyRepository venueRepository,
        CancellationToken cancellationToken = default)
    {
        var venueExists = await venueRepository.ExistsAsync(venueId, cancellationToken);
        if (!venueExists)
            return Result.Failure<TimeSlot>(DomainErrors.Venue.NotFound(venueId));

        return new TimeSlot(id, staffId, venueId, date, new List<Interval>());
    }
    
    public Result AddInterval(TimeSpan start, TimeSpan end)
    {
        var intervalResult = Interval.Create(start, end);
        if (intervalResult.IsFailure)
            return intervalResult;

        var newInterval = intervalResult.Value;

        foreach (var existingInterval in Intervals)
        {
            if (existingInterval.Overlaps(newInterval))
            {
                return Result.Failure(DomainErrors.TimeSlot.IntervalsOverlap);
            }
        }

        Intervals.Add(newInterval);
        return Result.Success();
    }


    public Result UpdateIntervals(List<Interval> intervals)
    {
        for (int i = 0; i < intervals.Count; i++)
        {
            for (int j = i + 1; j < intervals.Count; j++)
            {
                if (intervals[i].Overlaps(intervals[j]))
                {
                    return Result.Failure(DomainErrors.TimeSlot.IntervalsOverlap);
                }
            }
        }

        Intervals = intervals;
        return Result.Success();
    }
}