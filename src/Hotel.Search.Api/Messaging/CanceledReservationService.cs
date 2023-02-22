using Hotel.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using Hotel.Search.Api.Contexts;
using Hotel.Common.Messaging.Events;

namespace Hotel.Search.Api.Messaging;

public record CanceledReservationEvent : CanceledEvent
{
  public CanceledReservationEvent(int Id) : base(Id)  {  }
}

public class CanceledReservationService : BaseBackgroundService<CanceledReservationEvent>
{
  public CanceledReservationService(IMessageBus bus, IServiceProvider sp) : base(bus, sp) { }

  protected override async Task ProcessEventAsync(CanceledReservationEvent item)
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
