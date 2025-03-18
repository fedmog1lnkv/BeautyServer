using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Records.Commands.CreateRecord;

public record CreateRecordCommand(
    Guid UserId,
    Guid StaffId,
    Guid ServiceId,
    DateTime StartTimestamp,
    DateTime? EndTimeStamp) : ICommand<Result>;