using Hotel.Common;
using Hotel.Common.Messaging;
using Hotel.Search.Api.Rooms;

namespace Hotel.Search.Api.Messaging;

public abstract class BaseBackgroundService<TEvent> : BackgroundService where TEvent : class
{
  private readonly IMessageBus bus;
  protected readonly IServiceProvider serviceProvider;

  protected BaseBackgroundService(IMessageBus bus, IServiceProvider serviceProdiver)
  {
    this.bus = bus;
    this.serviceProvider = serviceProdiver;
  }
  protected abstract Task ProcessEventAsync(TEvent arg);

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await bus.SubscribeAsync<TEvent>(string.Empty, ProcessAndClearCacheAsync, stoppingToken);
  }

  private async Task ProcessAndClearCacheAsync(TEvent arg)
  {
    await ProcessEventAsync(arg);
    using var serviceScope = serviceProvider.CreateScope();
    var cache = serviceScope.ServiceProvider.GetRequiredService<ICacheManagement>();
    if (cache is null) return;

    await cache.RemoveAsync(AvailableRoomsQuery.CacheKey, default);
  }
}
