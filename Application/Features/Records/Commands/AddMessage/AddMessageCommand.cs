using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Records.Commands.AddMessage;

public record AddMessageCommand(
    Guid RecordId,
    Guid SenderId,
    string Text) : ICommand<Result>;