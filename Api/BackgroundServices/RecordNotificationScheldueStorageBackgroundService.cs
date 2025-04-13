using Api.Storages;
using Api.Utils;
using Application.Features.Records.Commands.NotifyScheduleRecords;
using Application.Features.Records.Queries.GetApprovedRecords;
using MediatR;
using Serilog;

namespace Api.BackgroundServices;

public class RecordNotificationScheldueStorageBackgroundService(
    IServiceScopeFactory scopeFactory,
    NotificationSchedulerStorage storage) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await RepeatHelper.RepeatAction(DoWork, TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10), cancellationToken);
    }

    private async Task DoWork()
    {
        using var scope = scopeFactory.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var now = DateTime.UtcNow;

        var dueNotifications = storage.GetReadyNotifications(now);

        foreach (var notification in dueNotifications)
        {
            await notification.Callback();
            storage.Remove(notification.RecordId);
        }

        var query = new GetApprovedRecordsQuery();
        var result = await sender.Send(query);

        if (result.IsFailure)
        {
            Log.Error(result.Error.Message);
            return;
        }

        var records = result.Value.Where(r => r.User.Settings.ReceiveOrderNotifications).ToList();

        foreach (var record in records)
        {
            if (storage.Contains(record.Id))
                continue;

            // TODO : Debug
            var triggerTime = record.StartTimestamp.ToUniversalTime().DateTime.AddMinutes(-30);
            if (triggerTime > now)
            {
                var startTime = record.StartTimestamp.ToString("HH:mm");
                var endTime = record.EndTimestamp.ToString("HH:mm");

                storage.Add(
                    new ScheduledNotification
                    {
                        RecordId = record.Id,
                        TriggerAt = triggerTime,
                        Callback = async () => await SendReminder(
                            sender,
                            record.Id,
                            record.User.Settings.FirebaseToken!,
                            "Напоминание о записи через 30 минут",
                            $"{record.Venue.Name} {startTime} - {endTime}")
                    });
            }
        }
    }

    private async Task SendReminder(
        ISender sender,
        Guid recordId,
        string deviceToken,
        string title,
        string message)
    {
        var command = new NotifyScheduleRecordsCommand(recordId, deviceToken, title, message);
        await sender.Send(command);
    }
}