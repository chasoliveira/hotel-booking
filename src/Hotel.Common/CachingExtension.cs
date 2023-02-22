using Hotel.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public record RedisOption
{
  public const string SectionName = "Redis";
  public string Endpoint { get; init; } = String.Empty;
  public string Password { get; init; } = String.Empty;
  public static implicit operator ConfigurationOptions(RedisOption redisOption)
    => new ConfigurationOptions
    {
      Password = redisOption.Password,
      EndPoints = { redisOption.Endpoint }
    };
}

[ExcludeFromCodeCoverage]
public static class CachingExtension
{
  public static void AddCaching(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddScoped<ICacheManagement, CacheManagement>();

    services.AddStackExchangeRedisCache(setup =>
    {
      ConfigurationOptions options = GetRedisConfiguration(services, configuration);
      setup.ConnectionMultiplexerFactory =
        () => Task.FromResult<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));
    });
  }

  private static ConfigurationOptions GetRedisConfiguration(IServiceCollection services, IConfiguration configuration)
  {
    var redisOption = new RedisOption();
     configuration.GetSection(RedisOption.SectionName).Bind(redisOption);

    ArgumentNullException.ThrowIfNull(redisOption);
    HotelException.ThrowIfNullOrEmpty(redisOption.Endpoint);
    HotelException.ThrowIfNullOrEmpty(redisOption.Password);

    return redisOption;
  }
}
