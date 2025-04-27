using Domain.Enums;
using Domain.Primitives;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class RecordStatusLog : Entity
{
    private RecordStatusLog(
        Guid id,
        Guid recordId,
        RecordStatusChange statusChange,
        RecordStatusLogDescription description,
        DateTime timestamp)
        : base(id)
    {
        RecordId = recordId;
        StatusChange = statusChange;
        Description = description;
        Timestamp = timestamp;
    }

#pragma warning disable CS8618
    private RecordStatusLog() { }
#pragma warning restore CS8618

    public Guid RecordId { get; private set; }
    public RecordStatusChange StatusChange { get; private set; }
    public RecordStatusLogDescription Description { get; private set; }
    public DateTime Timestamp { get; private set; }

    public static Result<RecordStatusLog> Create(
        Guid id,
        Guid recordId,
        RecordStatusChange statusChange,
        string description,
        DateTime timestamp)
    {
        var createDescriptionResult = RecordStatusLogDescription.Create(description);
        if (createDescriptionResult.IsFailure)
            return Result.Failure<RecordStatusLog>(createDescriptionResult.Error);

        return new RecordStatusLog(
            id,
            recordId,
            statusChange,
            createDescriptionResult.Value,
            timestamp);
    }
}