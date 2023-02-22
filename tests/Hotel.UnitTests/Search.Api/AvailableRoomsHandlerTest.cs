using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Hotel.Search.Api.Contexts;
using Hotel.Search.Api.Contexts.Entities;
using Hotel.Search.Api.Rooms;
using Hotel.Common;

namespace Hotel.UnitTests;

public class AvailableRoomsHandlerTest
{
  private readonly Mock<ICacheManagement> _cacheManagementMock = new();

  private AvailableRoomsQuery GetRequest()
  {
    var startAt = DateTime.Today.ToString("yyy-MM-dd");
    var endAt = DateTime.Today.AddDays(3).ToString("yyy-MM-dd");
    return new AvailableRoomsQuery(startAt, endAt);
  }

  [Fact]
  public async Task Handle_ShouldReturn_Empty()
  {
    //Arrange
    var contextInMemory = ContextFactory.NewInMemory<SearchContext>();
    var request = GetRequest();
    var handler = new AvailableRoomsHandler(_cacheManagementMock.Object, contextInMemory);

    //Act
    var result = await handler.Handle(request, default);

    //Assert
    result.Should().BeEmpty();

    _cacheManagementMock.Verify(c => c.GetAsync<List<ReservedProjection>>(It.IsAny<string>(), default), Times.Once);
    _cacheManagementMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<It.IsAnyType>, default), Times.Never);
  }

  [Fact]
  public async Task Handle_ShouldReturn_Items_From_Cache()
  {
    //Arrange
    var contextInMemory = ContextFactory.NewInMemory<SearchContext>();
    var request = GetRequest();
    var handler = new AvailableRoomsHandler(_cacheManagementMock.Object, contextInMemory);

    _cacheManagementMock
        .Setup(c => c.GetAsync<List<ReservedProjection>>(It.IsAny<string>(), default))
        .ReturnsAsync(new List<ReservedProjection>() {
            new(1,"CachedValue1", DateTime.Now, DateTime.Now.AddDays(3)),
            new(1,"CachedValue1", DateTime.Now.AddDays(4), DateTime.Now.AddDays(7)),
            new(2,"CachedValue2", DateTime.Now.AddDays(11), DateTime.Now.AddDays(14))
         });
    //Act
    var result = await handler.Handle(request, default);

    //Assert
    result.Should().SatisfyRespectively(
        value1 =>
        {
          value1.Id.Should().Be(1);
          value1.Description.Should().Be("CachedValue1");
        },
        value2 =>
        {
          value2.Id.Should().Be(2);
          value2.Description.Should().Be("CachedValue2");
        });
    _cacheManagementMock.Verify(c => c.GetAsync<List<ReservedProjection>>(It.IsAny<string>(), default), Times.Once);
  }


  [Fact]
  public async Task Handle_ShouldReturn_Items_From_Database()
  {
    //Arrange
    var distributedCacheMock = new Mock<IDistributedCache>();

    var contextInMemory = ContextFactory.NewInMemory<SearchContext>();
    var request = GetRequest();
    await contextInMemory.Rooms.AddRangeAsync(
        new List<Room> {
            new Room{ Id=1, Description="CachedValue1",
            Reservations = new List<Reservation> { new Reservation{ UserName = "chasoliveira", StartAt=DateTime.Now, EndAt= DateTime.Now }}},
            new Room{ Id=2, Description="CachedValue2",
            Reservations = new List<Reservation> { new Reservation{ UserName = "chasoliveira", StartAt=DateTime.Now, EndAt= DateTime.Now }}}
         }
    );

    await contextInMemory.SaveChangesAsync();
    distributedCacheMock.Setup(d => d.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.UTF8.GetBytes(""));
    var handler = new AvailableRoomsHandler(new CacheManagement(distributedCacheMock.Object), contextInMemory);

    //Act
    var result = await handler.Handle(request, default);

    //Assert
    result.Should().SatisfyRespectively(
        value1 =>
        {
          value1.Id.Should().Be(1);
          value1.Description.Should().Be("CachedValue1");
        },
        value2 =>
        {
          value2.Id.Should().Be(2);
          value2.Description.Should().Be("CachedValue2");
        });

    distributedCacheMock.Verify(d => d.GetAsync(It.IsAny<string>(), default), Times.Once);
    distributedCacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
  }
}