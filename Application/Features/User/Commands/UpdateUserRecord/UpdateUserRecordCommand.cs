using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.User.Commands.UpdateUserRecord;

public record UpdateUserRecordCommand(
    Guid UserId,
    Guid RecordId,
    string? Status,
    byte? Rating,
    string? Comment) :
    ICommand<Result>;