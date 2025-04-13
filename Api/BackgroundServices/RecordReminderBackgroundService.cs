using Api.Storages;
using Api.Utils;
using Application.Features.Records.Commands.NotifyScheduleRecords;
using Application.Features.Records.Queries.GetApprovedRecords;
using MediatR;
using Serilog;

namespace Api.BackgroundServices;

public class RecordReminderBackgroundService(
    IServiceScopeFactory scopeFactory,
    NotificationSchedulerStorage storage) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await RepeatHelper.RepeatAction(DoWork, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), cancellationToken);
    }

    private async Task DoWork()
    {
        using var scope = scopeFactory.CreateScope();

        var now = DateTime.UtcNow;

        var dueNotifications = storage.GetReadyNotifications(now);

        foreach (var notification in dueNotifications)
        {
            await notification.Callback();
            storage.Remove(notification.RecordId);
        }
    }
}