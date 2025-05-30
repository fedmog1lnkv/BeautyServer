namespace Domain.IntegrationEvents.Record;

public sealed record RecordDeleteMessageEvent(
    Guid Id,
    Guid RecordId,
    Guid MessageId) : IntegrationEvent(Id);