using Hotel.Search.Api;
using Hotel.Search.Api.Contexts;
using Hotel.Search.Api.Contexts.Entities;
using Hotel.Search.Api.Rooms;

namespace Hotel.IntegrationTests;

public class SearchApiIntegrationTest : IClassFixture<TestWebApplicationFactory<Program, SearchContext>>
{
  private readonly TestWebApplicationFactory<Program, SearchContext> _factory;

  public SearchApiIntegrationTest(TestWebApplicationFactory<Program, SearchContext> factory)
  {
    _factory = factory;
  }
  private (string StartAt, string EndAt) GetPatameter()
  {
    var startAt = DateTime.Today.ToString("yyy-MM-dd");
    var endAt = DateTime.Today.AddDays(3).ToString("yyy-MM-dd");
    return (startAt, endAt);
  }

  [Fact]
  public async Task SearchRooms_Should_Return_EmptyList_OK200()
  {
    //Arrange
    var client = _factory.CreateClient();
    var (startAt, endAt) = GetPatameter();

    //Act
    var response = await client.GetFromJsonAsync<IEnumerable<RoomViewModel>>($"/api/rooms?startAt={startAt}&endAt={endAt}");

    //Assert
    response.Should().NotBeNull();
    response.Should().BeEmpty();
  }

  [Fact]
  public async Task SearchRooms_Should_Return_OneItem_OK200()
  {
    //Arrange
    var userName = "chasoliveira";
    var newRoom = new Room
    {
      Description = "JustOneRoom",
      Reservations = new List<Reservation> {
        new Reservation { UserName = userName, StartAt= DateTime.Now, EndAt = DateTime.Now.AddDays(15) }
      }
    };

    await _factory.AddEntityAsync(newRoom);

    var client = _factory.CreateClient();
    var (startAt, endAt) = GetPatameter();

    //Act
    var response = await client.GetFromJsonAsync<IEnumerable<RoomViewModel>>($"/api/rooms/?startAt={startAt}&endAt={endAt}");

    //Assert
    response.Should().NotBeNull();
    response.Should().SatisfyRespectively(room =>
    {
      room.Id.Should().Be(newRoom.Id);
      room.Description.Should().Be(newRoom.Description);
    });
  }
}
