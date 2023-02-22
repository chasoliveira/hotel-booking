using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Castle.Core.Internal;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Hotel.Common;

[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)]
namespace Hotel.UnitTests;

internal record Foo
{
  public Foo() { }
  public Foo(string prop) { Prop = prop; }
  public string? Prop { get; init; }
}

public class CacheManagementTest
{

  [Fact]
  public async Task ShouldReturn_Items_From_Cache()
  {
    //Arrange
    var value = new Foo("Hello");
    var distributedCacheMock = new Mock<IDistributedCache>();
    distributedCacheMock
      .Setup(d => d.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)));

    var cacheManagement = new CacheManagement(distributedCacheMock.Object);

    //Act
    var result = await cacheManagement.GetAsync<Foo>("someKey", default);

    //Assert
    result.Should().NotBeNull();
    result?.Prop.Should().Be("Hello");

    distributedCacheMock.Verify(d => d.GetAsync(It.IsAny<string>(), default), Times.Once);
    distributedCacheMock.Verify(d => d.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Never);
  }

  [Fact]
  public async Task ShouldReturn_Null_WhenCache_Has_No_Result()
  {
    //Arrange
    var distributedCacheMock = new Mock<IDistributedCache>();
    distributedCacheMock
      .Setup(d => d.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(Encoding.UTF8.GetBytes(string.Empty));


    var cacheManagement = new CacheManagement(distributedCacheMock.Object);

    //Act
    var result = await cacheManagement.GetAsync<Foo>("someKey", (CancellationToken)default);

    //Assert
    result.Should().BeNull();

    distributedCacheMock.Verify(d => d.GetAsync(It.IsAny<string>(), default), Times.Once);
    distributedCacheMock.Verify(d => d.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Never);
  }

  [Fact]
  public async Task ShouldReturn_Item_And_SetCache()
  {
    //Arrange
    var distributedCacheMock = new Mock<IDistributedCache>();
    var cacheManagement = new CacheManagement(distributedCacheMock.Object);

    //Act
    Func<Task> execution = ()=> cacheManagement.SetAsync<Foo>("someKey", new Foo("Hello"),default);

    //Assert
    await execution.Should().NotThrowAsync();

    distributedCacheMock.Verify(d => d.GetAsync(It.IsAny<string>(), default), Times.Never);
    distributedCacheMock.Verify(d => d.SetAsync("someKey", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
  }
}
