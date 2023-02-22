using EasyNetQ;

namespace Hotel.Common.Messaging.Events;

public abstract record BaseEventMessage
{
  public const string ExchangeName = "Hotel.Booking";
  public DateTime Timestamp => DateTime.Now;
  public string MessageType => GetType().Name;
}

[Queue(nameof(AddedEvent), ExchangeName = "Hotel.Booking.AddedEvent" )]
public record AddedEvent(int Id, int RoomId, string RoomDescription, string UserName, DateTime StartAt, DateTime EndAt) : BaseEventMessage;

[Queue(nameof(UpdatedEvent), ExchangeName = "Hotel.Booking.UpdatedEvent")]
public record UpdatedEvent(int Id, DateTime StartAt, DateTime EndAt) : BaseEventMessage;

[Queue(nameof(CanceledEvent), ExchangeName = "Hotel.Booking.CanceledEvent")]
public record CanceledEvent(int Id) : BaseEventMessage;

[Queue(nameof(ConfirmedEvent), ExchangeName = "Hotel.Booking.ConfirmedEvent")]
public record ConfirmedEvent(int Id) : BaseEventMessage;

[Queue(nameof(DeletedEvent), ExchangeName = "Hotel.Booking.DeletedEvent")]
public record DeletedEvent(int Id) : BaseEventMessage;