using MediatR;

namespace Hotel.Common.Notifications;

public class NotificationHandler : INotificationHandler<Notification>
{
    private readonly INotificationContext context;

    public NotificationHandler(INotificationContext context) =>
        this.context = context;

    public Task Handle(Notification notification, CancellationToken cancellationToken) =>
        Task.Run(() => context.Add(notification), cancellationToken);
}