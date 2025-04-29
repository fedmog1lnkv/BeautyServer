using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Records.Commands.AddMessage;

public record AddMessageCommand(
    Guid Id,
    Guid RecordId,
    Guid SenderId,
    string Text) : ICommand<Result>
{
    public AddMessageCommand(
        Guid RecordId,
        Guid SenderId,
        string Text) : this(Guid.NewGuid(), RecordId, SenderId, Text) { }
}