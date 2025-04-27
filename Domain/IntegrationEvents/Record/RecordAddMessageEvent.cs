namespace Domain.IntegrationEvents.Record;

public sealed record RecordAddMessageEvent(
    Guid Id,
    Guid SenderId,
    string Message,
    DateTime CreatedAt) : IntegrationEvent(Id);