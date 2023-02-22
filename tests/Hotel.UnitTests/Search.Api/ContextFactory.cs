using Microsoft.EntityFrameworkCore;

namespace Hotel.UnitTests;

internal static class ContextFactory
{
  public static TContext NewInMemory<TContext>() where TContext : DbContext
  {
    var guid = Guid.NewGuid().ToString();
    object options = new DbContextOptionsBuilder<TContext>()
              .UseInMemoryDatabase(databaseName: $"Db_{guid}")
              .Options;
    return (TContext)Activator.CreateInstance(typeof(TContext), options)!;
  }
}
