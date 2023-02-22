using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.Common.Messaging;

public static class CommonMessageBusExtensions
{
  public static void AddMessagingBus(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddSingleton<IMessageBus, MessageBus>();
  }
}