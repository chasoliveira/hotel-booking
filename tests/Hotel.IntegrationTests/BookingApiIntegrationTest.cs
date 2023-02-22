using System.Net;
using Hotel.Booking.Api;
using Hotel.Booking.Api.Bookings;
using Hotel.Booking.Api.Bookings.Commands;
using Hotel.Booking.Api.Contexts;

namespace Hotel.IntegrationTests;

public class BookingApiIntegrationTest : IClassFixture<TestWebApplicationFactory<Program, BookingContext>>
{
  private readonly TestWebApplicationFactory<Program, BookingContext> factory;

  public BookingApiIntegrationTest(TestWebApplicationFactory<Program, BookingContext> factory)
  {
    this.factory = factory;
  }
  private async Task<(
    NewBookingCommand Command,
    string CreatedContent,
    HttpStatusCode StatusCode,
    BookingViewModel? fetchedCreated,
    HttpClient Client
    )> PostAndGetAsync()
  {
    //Arrange
    var userName = "chasoliveira";
    var room = await factory.AddEntityAsync(new Booking.Api.Contexts.Entities.Room { Description = "Room with two beds" });
    var command = new NewBookingCommand(room.Id, userName, DateTime.Now, DateTime.Now.AddDays(1));
    var client = factory.CreateClient();
    var createItem = await client.PostAsJsonAsync<NewBookingCommand>($"/api/bookings?userName={userName}", command);

    var createdContent = await createItem.Content.ReadAsStringAsync();

    //Act
    var response = await client.GetFromJsonAsync<BookingViewModel>($"{createItem.Headers.Location}?userName={userName}");

    return (command, createdContent, createItem.StatusCode, response, client);
  }

  [Fact]
  public async Task GetById_Should_Return_Success()
  {
    //Arrange
    //Act
    var (command, createdContent, statusCode, bookingCreated, _) = await PostAndGetAsync();

    //Assert
    statusCode.Should().Be(HttpStatusCode.Created);
    bookingCreated.Should().NotBeNull();
    bookingCreated.Id.Should().BeGreaterThan(0);
    bookingCreated.StartAt.Should().Be(command.StartAt);
    bookingCreated.EndAt.Should().Be(command.EndAt);
    bookingCreated.IsCanceled.Should().BeFalse();
    bookingCreated.IsConfirmed.Should().BeFalse();
    bookingCreated.Room.Should().Be("Room with two beds");
    createdContent.Should().BeEmpty();
  }

  [Fact]
  public async Task GetById_Should_Return_BadRequest()
  {
    //Arrange
    var userName = "chasoliveira";
    var fakeRoomId = 2222;

    var command = new NewBookingCommand(fakeRoomId, userName, DateTime.Now, DateTime.Now.AddDays(1));
    var client = factory.CreateClient();

    //Act
    var response = await client.PostAsJsonAsync<NewBookingCommand>($"/api/bookings?userName={userName}", command);

    //Assert
    response.Should().NotBeNull();
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task Modify_Should_Return_Success()
  {
    //Arrange
    var (_, _, _, bookingCreated, client) = await PostAndGetAsync();
    var bookingId = bookingCreated.Id;
    var userName = bookingCreated.UserName;
    var updateCommand = new UpdateBookingCommand(bookingId, bookingCreated.UserName, DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(2));

    //Act
    var updatedItem = await client.PutAsJsonAsync<UpdateBookingCommand>($"/api/bookings/{bookingId}", updateCommand);

    //Assert
    updatedItem.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    var response = await client.GetFromJsonAsync<BookingViewModel>($"/api/bookings/{bookingId}?userName={userName}");
    response.StartAt.Should().Be(updateCommand.StartAt);
    response.EndAt.Should().Be(updateCommand.EndAt);
  }

  [Fact]
  public async Task Cancel_Should_Return_Success()
  {
    //Arrange
    var (_, _, _, bookingCreated, client) = await PostAndGetAsync();
    var bookingId = bookingCreated.Id;
    var userName = bookingCreated.UserName;
    var command = new CancelBookingCommand(bookingId, userName);

    //Act
    var patchedItem = await client.PatchAsJsonAsync<CancelBookingCommand>($"/api/bookings/{bookingId}/cancel", command);
    var bookingUpdated = await client.GetFromJsonAsync<BookingViewModel>($"/api/bookings/{bookingId}?userName={userName}");

    //Assert
    patchedItem.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    bookingCreated.IsCanceled.Should().BeFalse();
    bookingUpdated.IsCanceled.Should().BeTrue();
    bookingCreated.IsConfirmed.Should().BeFalse();
    bookingUpdated.IsConfirmed.Should().BeFalse();
  }

  [Fact]
  public async Task Confirm_Should_Return_Success()
  {
    //Arrange
    var (_, _, _, bookingCreated, client) = await PostAndGetAsync();
    var bookingId = bookingCreated.Id;
    var userName = bookingCreated.UserName;
    var command = new CancelBookingCommand(bookingId, userName);

    //Act
    var patchetedItem = await client.PatchAsJsonAsync<CancelBookingCommand>($"/api/bookings/{bookingId}/confirm", command);
    var bookingUpdated = await client.GetFromJsonAsync<BookingViewModel>($"/api/bookings/{bookingId}?userName={userName}");

    //Assert
    patchetedItem.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    bookingCreated.IsCanceled.Should().BeFalse();
    bookingUpdated.IsCanceled.Should().BeFalse();
    bookingCreated.IsConfirmed.Should().BeFalse();
    bookingUpdated.IsConfirmed.Should().BeTrue();
  }

  [Fact]
  public async Task Delete_Should_Return_Success()
  {
    //Arrange
    var (_, _, _, bookingCreated, client) = await PostAndGetAsync();
    var bookingId = bookingCreated.Id;
    var userName = bookingCreated.UserName;

    //Act
    var deletedItem = await client.DeleteAsync($"/api/bookings/{bookingId}?userName={userName}");
    var getBookingDeleted = await client.GetAsync($"/api/bookings/{bookingId}?userName={userName}");

    //Assert
    deletedItem.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    getBookingDeleted.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
  }
}