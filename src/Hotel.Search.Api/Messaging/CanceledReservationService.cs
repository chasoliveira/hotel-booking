using Hotel.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using Hotel.Search.Api.Contexts;
using Hotel.Common.Messaging.Events;

namespace Hotel.Search.Api.Messaging;

public class CanceledReservationService : BaseBackgroundService<CanceledEvent>
{
  public CanceledReservationService(IMessageBus bus, IServiceProvider sp) : base(bus, sp) { }

  protected override async Task ProcessEventAsync(CanceledEvent item)
  {
    if (item is null) return;
    using var serviceScope = serviceProvider.CreateScope();
    var context = serviceScope.ServiceProvider.GetService<SearchContext>()!;

    var existingReservation = await context.Reservations.FirstOrDefaultAsync(r => r.Id == item.Id && !r.Canceled);
    if (existingReservation is null) return;

    existingReservation.Canceled = true;

    await context.SaveChangesAsync();
  }
}
