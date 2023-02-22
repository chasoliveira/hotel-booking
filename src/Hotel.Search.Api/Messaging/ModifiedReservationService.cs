using Microsoft.EntityFrameworkCore;
using Hotel.Search.Api.Contexts;
using Hotel.Common.Messaging.Events;
using Hotel.Common.Messaging;

namespace Hotel.Search.Api.Messaging;
public record ModifiedReservationEvent : UpdatedEvent
{
  public ModifiedReservationEvent(int Id, DateTime StartAt, DateTime EndAt)
    : base(Id, StartAt, EndAt) { }
}

public class ModifiedReservationService : BaseBackgroundService<ModifiedReservationEvent>
{
  public ModifiedReservationService(IMessageBus bus, IServiceProvider sp) : base(bus, sp) { }

  protected override async Task ProcessEventAsync(ModifiedReservationEvent item)
  {
    if (item is null) return;

    using var serviceScope = serviceProvider.CreateScope();
    var context = serviceScope.ServiceProvider.GetService<SearchContext>()!;

    var existingReservation = await context.Reservations.FirstOrDefaultAsync(r => r.Id == item.Id);
    if (existingReservation is null) return;

    existingReservation.StartAt = item.StartAt;
    existingReservation.EndAt = item.EndAt;

    context.Reservations.Update(existingReservation);

    await context.SaveChangesAsync();
  }
}