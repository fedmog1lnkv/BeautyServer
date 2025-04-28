using Application.Features.User.Commands.UpdateUserRecord.Dto;
using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.User.Commands.UpdateUserRecord;

public record UpdateUserRecordCommand(
    Guid UserId,
    Guid RecordId,
    string? Status,
    RecordReviewDto? Review) :
    ICommand<Result>;