using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Hotel.Booking.Api.Bookings;
using Hotel.Booking.Api.Bookings.Commands;
using Hotel.Booking.Api.Contexts;
using Hotel.Common.Messaging;
using Hotel.Common.Notifications;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Hotel.UnitTests.Booking.Api;

public class BookingCommandHandlerTest
{

  private readonly Mock<IMessageBus> messageBus = new();
  private readonly Mock<IServiceProvider> serviceProvider = new();
  private readonly Mock<INotificationContext> notification = new();
  private readonly Mock<IValidator<NewBookingCommand>> newBookingValidator = new();

  private (BookingCommandHandler, BookingContext) Sut()
  {
    var context = ContextFactory.NewInMemory<BookingContext>();
    serviceProvider.Setup(s => s.GetService(typeof(IValidator<NewBookingCommand>))).Returns(newBookingValidator.Object);

    return (new BookingCommandHandler(messageBus.Object, serviceProvider.Object, notification.Object, context), context);
  }
  [Fact]
  public async Task Handle_NewBookingCommand_Return_True()
  {
    //Arrange
    var (sut, ctx) = Sut();
    var room = ctx.Rooms.Add(new Hotel.Booking.Api.Contexts.Entities.Room { Description = "Room" });
    ctx.SaveChanges();

    var request = new NewBookingCommand(room.Entity.Id, "chasoliveira", DateTime.Now, DateTime.Now.AddDays(4));
    newBookingValidator.Setup(v => v.Validate(request)).Returns(new ValidationResult());
    //Act
    var (success, bookingId) = await sut.Handle(request, default);

    //Assert
    success.Should().BeTrue();
    bookingId.Should().BeGreaterThan(0);

    (await ctx.Bookings.AnyAsync(b => b.Id == bookingId)).Should().BeTrue();

    notification.Verify(n => n.Add(It.IsAny<Notification>()), Times.Never);
    messageBus.Verify(n => n.PublishAsync(It.IsAny<NewBookingEvent>(), default), Times.Once);
  }

  [Fact]
  public async Task Handle_NewBookingCommand_Return_False_When_Validation_Fails()
  {
    //Arrange
    var request = new NewBookingCommand(123, "chasoliveira", DateTime.Now, DateTime.Now.AddDays(4));
    var messageError = $"Some Validation Error";
    newBookingValidator.Setup(v => v.Validate(request)).Returns(new ValidationResult(new[] { new ValidationFailure("PropName", messageError) }));
    var (sut, _) = Sut();

    //Act
    var (success, bookingId) = await sut.Handle(request, default);

    //Assert
    success.Should().BeFalse();
    bookingId.Should().BeNull();
    notification.Verify(n => n.Add(It.Is<Notification>(v => v.Message.Contains(messageError))), Times.Once);
    messageBus.Verify(n => n.PublishAsync(It.IsAny<NewBookingEvent>(), default), Times.Never);
  }

  [Fact]
  public async Task Handle_NewBookingCommand_Return_False_When_There_IsNo_Room()
  {
    //Arrange
    var request = new NewBookingCommand(123, "chasoliveira", DateTime.Now, DateTime.Now.AddDays(4));
    var messageError = $"The room '{request.RoomId}' is not valid!";
    newBookingValidator.Setup(v => v.Validate(request)).Returns(new ValidationResult());

    var (sut, _) = Sut();
    //Act
    var (success, bookingId) = await sut.Handle(request, default);

    //Assert
    success.Should().BeFalse();
    bookingId.Should().BeNull();
    notification.Verify(n => n.Add(It.Is<Notification>(v => v.Message.Contains(messageError))), Times.Once);
    messageBus.Verify(n => n.PublishAsync(It.IsAny<NewBookingEvent>(), default), Times.Never);
  }
}