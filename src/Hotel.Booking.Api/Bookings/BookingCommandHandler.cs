using FluentValidation;
using Hotel.Booking.Api.Bookings.Commands;
using Hotel.Booking.Api.Contexts;
using Hotel.Common.Messaging;
using Hotel.Common.Messaging.Events;
using Hotel.Common.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Api.Bookings;

public class BookingCommandHandler :
  IRequestHandler<NewBookingCommand, (bool, int?)>,
  IRequestHandler<UpdateBookingCommand, bool>,
  IRequestHandler<CancelBookingCommand, bool>,
  IRequestHandler<ConfirmBookingCommand, bool>,
  IRequestHandler<DeleteBookingCommand, bool>
{
  private readonly IMessageBus bus;
  private readonly IServiceProvider serviceProvider;
  private readonly INotificationContext notification;
  private readonly BookingContext context;

  public BookingCommandHandler(IMessageBus bus, IServiceProvider serviceProvider, INotificationContext notification, BookingContext context)
  {
    this.bus = bus;
    this.serviceProvider = serviceProvider;
    this.notification = notification;
    this.context = context;
  }

  public async Task<(bool, int?)> Handle(NewBookingCommand request, CancellationToken token)
  {
    if (!IsValid(request)) return (false, null);

    var existingRoom = await FirstOrDefaultRoomAsync(request.RoomId);
    if (existingRoom is null) return (false, null);

    Contexts.Entities.Booking newBooking = request;
    newBooking.Room = existingRoom;

    context.Add(newBooking);
    await context.SaveChangesAsync(token);

    NewBookingEvent message = newBooking;
    await bus.PublishAsync<AddedEvent>(message, token);

    return (true, newBooking.Id);
  }

  private async Task<Contexts.Entities.Room?> FirstOrDefaultRoomAsync(int id)
  {
    var existingRoom = await context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
    if (existingRoom is { }) return existingRoom;

    notification.Add(new Notification($"The room '{id}' is not valid!"));
    return null;
  }

  private bool IsValid(NewBookingCommand request)
  {
    var validator = serviceProvider.GetService<IValidator<NewBookingCommand>>()!;
    var validationResult = validator.Validate(request);

    if (validationResult.IsValid) return true;

    notification.Add(validationResult.Errors.Select(er => new Notification(er.ErrorMessage)).ToArray());
    return false;
  }

  public async Task<bool> Handle(UpdateBookingCommand request, CancellationToken token)
  {
    var booking = await context.Bookings.FirstOrDefaultAsync(r => r.Id == request.Id);
    if (booking is null)
    {
      notification.Add(new Notification($"The booking '{request.Id}' is not valid!"));
      return false;
    }

    var validator = serviceProvider.GetService<IValidator<UpdateBookingCommand>>()!;
    var validationResult = validator.Validate(request);
    if (!validationResult.IsValid)
    {
      notification.Add(validationResult.Errors.Select(er => new Notification(er.ErrorMessage)).ToArray());
      return false;
    }

    booking.StartAt = request.StartAt;
    booking.EndAt = request.EndAt;

    await context.SaveChangesAsync(token);

    UpdateBookingEvent message = booking;
    await bus.PublishAsync<UpdatedEvent>(message, token);

    return true;
  }

  public async Task<bool> Handle(CancelBookingCommand request, CancellationToken token)
  {
    var booking = await context.Bookings.FirstOrDefaultAsync(r => r.Id == request.Id);
    if (booking is null)
    {
      notification.Add(new Notification($"The booking '{request.Id}' is not valid!"));
      return false;
    }

    booking.IsCanceled = true;
    await context.SaveChangesAsync(token);

    await bus.PublishAsync(new CanceledEvent(request.Id), token);

    return true;
  }

  public async Task<bool> Handle(ConfirmBookingCommand request, CancellationToken token)
  {
    var booking = await context.Bookings.FirstOrDefaultAsync(r => r.Id == request.Id);
    if (booking is null || booking.IsCanceled)
    {
      notification.Add(new Notification($"The booking '{request.Id}' is not valid!"));
      return false;
    }

    booking.IsConfirmed = true;
    await context.SaveChangesAsync(token);

    await bus.PublishAsync(new ConfirmedEvent(request.Id), token);

    return true;
  }

  public async Task<bool> Handle(DeleteBookingCommand request, CancellationToken token)
  {
    var booking = await context.Bookings.FirstOrDefaultAsync(r => r.Id == request.Id);
    if (booking is null)
    {
      notification.Add(new Notification($"The booking '{request.Id}' is not valid!"));
      return false;
    }

    context.Remove(booking);
    await context.SaveChangesAsync(token);

    await bus.PublishAsync(new DeletedEvent(request.Id), token);

    return true;
  }

}
