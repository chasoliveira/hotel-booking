using Hotel.Common.Notifications;

namespace Hotel.Common.Messaging;

public static class MessageBusExtensions
{
  public static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddMessagingBus(configuration);
    services.AddScoped<INotificationContext, NotificationContext>();
  }
}