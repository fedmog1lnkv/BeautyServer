using Application.Messaging.Command;
using Domain.Repositories.Utils;
using Domain.Shared;

namespace Application.Features.Records.Commands.NotifyScheduleRecords;

public class NotifyScheduleRecordsCommandHandler(INotificationRepository notificationRepository) :
    ICommandHandler<NotifyScheduleRecordsCommand, Result>
{
    public async Task<Result> Handle(NotifyScheduleRecordsCommand request, CancellationToken cancellationToken)
    {
        await notificationRepository.SendOrderNotificationAsync(
            request.RecordId,
            request.DeviceToken,
            request.Title,
            request.Message);

        return Result.Success();
    }
}