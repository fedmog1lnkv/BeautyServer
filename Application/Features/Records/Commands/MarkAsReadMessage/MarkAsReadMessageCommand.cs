using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Records.Commands.MarkAsReadMessage;

public record MarkAsReadMessageCommand(
    Guid RecordId,
    Guid ReaderId,
    List<Guid> MessageIds) : ICommand<Result>;