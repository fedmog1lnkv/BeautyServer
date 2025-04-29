namespace Domain.IntegrationEvents.Record;

public sealed record RecordStatusLogEvent(
    Guid Id,
    Guid RecordId,
    Guid LogId,
    string Status,
    string Description,
    DateTime Date) : IntegrationEvent(Id);