using Domain.Primitives;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class RecordMessage : Entity
{
    private RecordMessage(
        Guid id,
        Guid recordId,
        Guid senderId,
        RecordMessageContent message)
        : base(id)
    {
        RecordId = recordId;
        SenderId = senderId;
        Message = message;
        CreatedAt = DateTime.UtcNow;
    }

#pragma warning disable CS8618
    private RecordMessage() { }
#pragma warning restore CS8618

    public Guid RecordId { get; private set; }
    public Guid SenderId { get; private set; }
    public RecordMessageContent Message { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Result<RecordMessage> Create(
        Guid id,
        Guid recordId,
        Guid senderId,
        string message)
    {
        var messageResult = RecordMessageContent.Create(message);
        if (messageResult.IsFailure)
            return Result.Failure<RecordMessage>(messageResult.Error);

        return new RecordMessage(
            id,
            recordId,
            senderId,
            messageResult.Value);   
    }

    public Result MarkAsRead(DateTime readAt)
    {
        if (IsRead)
            return Result.Success();
        
        IsRead = true;
        ReadAt = readAt;
        return Result.Success();
    }
}