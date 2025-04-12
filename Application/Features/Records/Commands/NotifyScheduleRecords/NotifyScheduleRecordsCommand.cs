using Application.Messaging.Command;
using Domain.Shared;

namespace Application.Features.Records.Commands.NotifyScheduleRecords;

public record NotifyScheduleRecordsCommand(
    Guid RecordId,
    string DeviceToken,
    string Title,
    string Message) : ICommand<Result>;