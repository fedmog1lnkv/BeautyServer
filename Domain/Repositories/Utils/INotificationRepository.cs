using Domain.Repositories.Base;

namespace Domain.Repositories.Utils;

public interface INotificationRepository
{
    Task SendOrderNotificationAsync(
        Guid recordId,
        string deviceToken,
        string title,
        string message);
}