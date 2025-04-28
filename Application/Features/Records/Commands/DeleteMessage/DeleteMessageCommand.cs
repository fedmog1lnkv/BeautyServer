using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Records.Commands.DeleteMessage;

public record DeleteMessageCommand(
    Guid InitiatorId,
    Guid RecordId,
    Guid MessageId) : ICommand<Result>;