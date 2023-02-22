using MediatR;

namespace Hotel.Booking.Api.Bookings.Commands;

public record NewBookingCommand(int RoomId, string UserName, DateTime StartAt, DateTime EndAt) : IRequest<(bool, int?)>
{
  public static implicit operator Contexts.Entities.Booking(NewBookingCommand booking)
    => new Contexts.Entities.Booking
    {
      RoomId = booking.RoomId,
      UserName = booking.UserName,
      StartAt = booking.StartAt,
      EndAt = booking.EndAt
    };
}

public record UpdateBookingCommand(int Id, string UserName, DateTime StartAt, DateTime EndAt) : IRequest<bool>;
public record CancelBookingCommand(int Id, string UserName) : IRequest<bool>;
public record ConfirmBookingCommand(int Id, string UserName) : IRequest<bool>;
public record DeleteBookingCommand(int Id, string UserName) : IRequest<bool>;
