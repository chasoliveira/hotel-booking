using Hotel.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using Hotel.Search.Api.Contexts;
using Hotel.Search.Api.Contexts.Entities;
using Hotel.Common.Messaging.Events;

namespace Hotel.Search.Api.Messaging;

public record NewReservationEvent: AddedEvent
{
  public NewReservationEvent(int Id, int RoomId, string RoomDescription, string UserName, DateTime StartAt, DateTime EndAt) 
    : base(Id, RoomId, RoomDescription, UserName, StartAt, EndAt)  {  }

  public static implicit operator Room(NewReservationEvent item)
    => new Room { ReferenceId = item.RoomId, Description = item.RoomDescription };
  public static implicit operator Reservation(NewReservationEvent item)
    => new Reservation { ReferenceId = item.Id, ReferenceRoomId = item.RoomId, UserName = item.UserName, StartAt = item.StartAt, EndAt = item.EndAt };
}

public class NewReservationService : BaseBackgroundService<NewReservationEvent>
{
  public NewReservationService(IMessageBus bus, IServiceProvider sp) : base(bus, sp) { }

  protected override async Task ProcessEventAsync(NewReservationEvent item)
  {
    if (item is null) return;

    using var serviceScope = serviceProvider.CreateScope();
    var context = serviceScope.ServiceProvider.GetService<SearchContext>()!;

    Reservation reservervation = item;
    var existingRoom = await context.Rooms.FirstOrDefaultAsync(r => r.Id == item.RoomId);

    reservervation.Room = existingRoom ?? item;

    await context.Reservations.AddAsync(reservervation);

    await context.SaveChangesAsync();
  }
}
