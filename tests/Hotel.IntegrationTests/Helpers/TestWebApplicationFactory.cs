using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hotel.Common;
using Hotel.Common.Messaging;

[assembly: InternalsVisibleTo("IntegrationTests.SearchApiIntegrationTest")]
namespace Hotel.IntegrationTests.Helpers;

public class TestWebApplicationFactory<TProgram, TContext>
  : WebApplicationFactory<TProgram>
    where TProgram : class
    where TContext : DbContext
{

public TestWebApplicationFactory()
{
  Environment.SetEnvironmentVariable("xUnit", "true");
}
  protected override IHost CreateHost(IHostBuilder builder)
  {
    Console.WriteLine("Program FullName: {0}", typeof(TProgram));
    Console.WriteLine("Context FullName: {0}", typeof(TContext));

    builder.ConfigureServices(services =>
    {
      services.RemoveInjections(typeof(DbContextOptions<TContext>));
      services.RemoveInjections(typeof(TContext));
      services.RemoveInjections(typeof(ICacheManagement));
      services.RemoveInjections(typeof(IMessageBus));

      services.AddSingleton<ICacheManagement, CacheManagementFake>();
      services.AddSingleton<IMessageBus, MessageBusFake>();

      services.AddDbContext<TContext>(options =>
      {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        options.UseSqlite($"Data Source={Path.Join(path, $"{typeof(TContext).Name}.db")}");
      });

    });
    var host = base.CreateHost(builder);
    using (var serviceScope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
      var context = serviceScope.ServiceProvider.GetService<TContext>()!;
      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();
    }
    return host;
  }

  public async Task<TEntity> AddEntityAsync<TEntity>(TEntity item) where TEntity : class
  {
    using var scope = this.Services.CreateScope();
    var db = (TContext)scope.ServiceProvider.GetService<TContext>()!;

    //if (db.Entry<TEntity>().Any()) return;

    await db.AddAsync(item);
    await db.SaveChangesAsync();
    return item;
  }
}
