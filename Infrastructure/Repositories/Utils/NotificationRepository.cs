using Domain.Repositories.Utils;
using FirebaseAdmin.Messaging;

namespace Infrastructure.Repositories.Utils;

public class NotificationRepository : INotificationRepository
{
    public async Task SendOrderNotificationAsync(
        Guid recordId,
        string deviceToken,
        string title,
        string message)
    {
        var deepLink = $"tagbeauty://orders/{recordId}";
        var messageToSend = new Message
        {
            Token = deviceToken,
            Notification = new Notification
            {
                Title = title,
                Body = message
            },
            Data = new Dictionary<string, string>
            {
                { "deep_link", deepLink }
            }
        };

        var response = await FirebaseMessaging.DefaultInstance.SendAsync(messageToSend);

        return;
    }
}