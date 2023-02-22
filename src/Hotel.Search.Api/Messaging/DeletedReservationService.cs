using Hotel.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using Hotel.Search.Api.Contexts;
using Hotel.Search.Api.Contexts.Entities;
using Hotel.Common.Messaging.Events;

namespace Hotel.Search.Api.Messaging;
public record DeletedReservationEvent : DeletedEvent
{
  public DeletedReservationEvent(int Id) : base(Id) { }
}
public class DeletedReservationService : BaseBackgroundService<DeletedEvent>
{
  public DeletedReservationService(IMessageBus bus, IServiceProvider sp) : base(bus, sp) { }
  
  protected override async Task ProcessEventAsync(DeletedEvent item)
  {
    if (item is null) return;
    using var serviceScope = serviceProvider.CreateScope();
    var context = serviceScope.ServiceProvider.GetService<SearchContext>()!;

    var existingReservation = await context.Reservations.FirstOrDefaultAsync(r => r.Id == item.Id);
    if (existingReservation is null) return;

    context.Remove(existingReservation);

    await context.SaveChangesAsync();
  }
}
