using Hotel.Common.Messaging.Events;

namespace Hotel.Booking.Api.Bookings;

public record NewBookingEvent : AddedEvent
{
  public NewBookingEvent(int Id, int RoomId, string RoomDescription, string UserName, DateTime StartAt, DateTime EndAt)
    : base(Id, RoomId, RoomDescription, UserName, StartAt, EndAt) { }

  public static implicit operator NewBookingEvent(Contexts.Entities.Booking booking)
    => new NewBookingEvent(booking.Id, booking.RoomId, booking.Room.Description, booking.UserName, booking.StartAt, booking.EndAt);
}
public record UpdateBookingEvent : UpdatedEvent
{
  public UpdateBookingEvent(int Id, DateTime StartAt, DateTime EndAt)
    : base(Id, StartAt, EndAt) { }

  public static implicit operator UpdateBookingEvent(Contexts.Entities.Booking booking)
    => new UpdateBookingEvent(booking.Id, booking.StartAt, booking.EndAt);
}
public record CancelBookingEvent : CanceledEvent
{
  public CancelBookingEvent(int Id) : base(Id)  {  }
}
public record ConfirmBookingEvent : ConfirmedEvent
{
  public ConfirmBookingEvent(int Id) : base(Id)  {  }
}

public record DeleteBookingEvent : DeletedEvent
{
  public DeleteBookingEvent(int Id) : base(Id)  {  }
}
