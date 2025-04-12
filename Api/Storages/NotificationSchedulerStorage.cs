namespace Api.Storages;

public class NotificationSchedulerStorage
{
    private readonly object _lock = new();
    private readonly SortedSet<ScheduledNotification> _notifications = new(new NotificationComparer());

    public void Add(ScheduledNotification notification)
    {
        lock (_lock)
        {
            _notifications.Add(notification);
        }
    }

    public List<ScheduledNotification> GetReadyNotifications(DateTime now)
    {
        lock (_lock)
        {
            var ready = _notifications
                .TakeWhile(n => n.TriggerAt <= now)
                .ToList();

            foreach (var item in ready)
                _notifications.Remove(item);

            return ready;
        }
    }
    
    public bool Contains(Guid id)
    {
        lock (_lock)
        {
            return _notifications.Any(n => n.RecordId == id);
        }
    }
    
    public void Remove(Guid id)
    {
        lock (_lock)
        {
            _notifications.RemoveWhere(n => n.RecordId == id);
        }
    }

    private class NotificationComparer : IComparer<ScheduledNotification>
    {
        public int Compare(ScheduledNotification? x, ScheduledNotification? y)
        {
            if (x == null || y == null) return 0;
            var timeCompare = x.TriggerAt.CompareTo(y.TriggerAt);
            return timeCompare != 0 ? timeCompare : x.RecordId.CompareTo(y.RecordId);
        }
    }
}

public class ScheduledNotification
{
    public Guid RecordId { get; init; }
    public DateTime TriggerAt { get; init; }
    public Func<Task> Callback { get; init; } = default!;
}