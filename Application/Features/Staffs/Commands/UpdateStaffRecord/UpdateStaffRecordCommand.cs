using Application.Messaging.Command;
using Domain.Enums;
using Domain.Shared;

namespace Application.Features.Staffs.Commands.UpdateStaffRecord;

public record UpdateStaffRecordCommand(
    Guid InitiatorId,
    Guid RecordId,
    string? Status,
    string? Comment,
    DateTime? StartTimestamp,
    DateTime? EndTimeStamp) :
    ICommand<Result>;