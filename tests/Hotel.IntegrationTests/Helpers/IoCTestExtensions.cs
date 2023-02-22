using Microsoft.Extensions.DependencyInjection;

namespace Hotel.IntegrationTests.Helpers;

public static class IoCTestExtensions
{
  public static void RemoveInjections(this IServiceCollection services, Type type)
  {
    var descriptor = services.SingleOrDefault(d => d.ServiceType == type);
    if (descriptor != null)
      services.Remove(descriptor);
  }
}
