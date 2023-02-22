using EasyNetQ;

namespace Hotel.Common.Messaging.Events;

public abstract record BaseEventMessage
{
  public const string ExchangeName = "Hotel.Booking.Excange";
  public DateTime Timestamp => DateTime.Now;
  public string MessageType => GetType().Name;
}

[Queue(nameof(AddedEvent), ExchangeName = BaseEventMessage.ExchangeName)]
public record AddedEvent(int Id, int RoomId, string RoomDescription, string UserName, DateTime StartAt, DateTime EndAt) : BaseEventMessage;

[Queue(nameof(UpdatedEvent), ExchangeName = BaseEventMessage.ExchangeName)]
public record UpdatedEvent(int Id, DateTime StartAt, DateTime EndAt) : BaseEventMessage;

[Queue(nameof(CanceledEvent), ExchangeName = BaseEventMessage.ExchangeName)]
public record CanceledEvent(int Id) : BaseEventMessage;

[Queue(nameof(ConfirmedEvent), ExchangeName = BaseEventMessage.ExchangeName)]
public record ConfirmedEvent(int Id) : BaseEventMessage;

[Queue(nameof(DeletedEvent), ExchangeName = BaseEventMessage.ExchangeName)]
public record DeletedEvent(int Id) : BaseEventMessage;