using MediatR;

namespace Hotel.Common.Notifications;

public record Notification(string Message) : INotification;

public interface INotificationContext
{
  void Add(params Notification[] notification);
  bool Any();
  IReadOnlyList<Notification> ReadOnlyList();
}

public class NotificationContext : INotificationContext
{
  private readonly List<Notification> _notifications;
  public NotificationContext() => _notifications = new List<Notification>();

  public void Add(params Notification[] notification) => _notifications.AddRange(notification);

  public bool Any() => _notifications.Any();

  public IReadOnlyList<Notification> ReadOnlyList() => _notifications.AsReadOnly();
}
