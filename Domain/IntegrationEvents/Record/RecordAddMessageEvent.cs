namespace Domain.IntegrationEvents.Record;

public sealed record RecordAddMessageEvent(
    Guid Id,
    Guid RecordId,
    Guid SenderId,
    Guid MessageId,
    string Message,
    DateTime CreatedAt) : IntegrationEvent(Id);