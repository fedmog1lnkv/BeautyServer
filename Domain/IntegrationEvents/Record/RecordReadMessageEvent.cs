namespace Domain.IntegrationEvents.Record;

public sealed record RecordReadMessageEvent(
    Guid Id,
    Guid RecordId,
    Guid ReaderId,
    Guid MessageId) : IntegrationEvent(Id);