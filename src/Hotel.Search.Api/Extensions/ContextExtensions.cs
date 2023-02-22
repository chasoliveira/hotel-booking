using System.Diagnostics.CodeAnalysis;
using Hotel.Search.Api.Contexts;
using Microsoft.EntityFrameworkCore;

[ExcludeFromCodeCoverage]
public static class ContextExtensions
{
  public static void AddContextServices(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<SearchContext>(options
      => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

  }

  public static void EnsureMigrations(this IApplicationBuilder app)
  {
    if (Environment.GetEnvironmentVariable("xUnit") == "true") return;
    
    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
      var context = serviceScope.ServiceProvider.GetService<SearchContext>()!;
      if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
    }
  }
}