using Hotel.Common.Messaging;

namespace Hotel.Search.Api.Messaging;

public static class MessageBusExtensions
{
  public static void AddMessagingConsumer(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddMessagingBus(configuration);
    services.AddHostedService<NewReservationService>();
    services.AddHostedService<CanceledReservationService>();
    services.AddHostedService<ConfirmedReservationService>();
    services.AddHostedService<DeletedReservationService>();
    services.AddHostedService<ModifiedReservationService>();
  }
}