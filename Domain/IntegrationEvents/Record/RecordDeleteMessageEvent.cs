namespace Domain.IntegrationEvents.Record;

public sealed record RecordDeleteMessageEvent(
    Guid Id,
    Guid MessageId) : IntegrationEvent(Id);