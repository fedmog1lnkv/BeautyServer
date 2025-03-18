using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public class Interval : ValueObject
{
    public TimeSpan Start { get; }
    public TimeSpan End { get; }

    private Interval(TimeSpan start, TimeSpan end)
    {
        Start = start;
        End = end;
    }

    public static Result<Interval> Create(TimeSpan start, TimeSpan end)
    {
        if (start >= end)
        {
            return Result.Failure<Interval>(DomainErrors.TimeSlot.IntervalsOverlap);
        }

        return new Interval(start, end);
    }

    public bool Overlaps(Interval other)
    {
        return Start < other.End && End > other.Start;
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Start;
        yield return End;
    }
}
