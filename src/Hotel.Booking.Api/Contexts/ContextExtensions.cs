using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Api.Contexts;

[ExcludeFromCodeCoverage]
public static class ContextExtensions
{
  public static void AddContextServices(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<BookingContext>(options
      => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

  }

  public static void EnsureMigrations(this IApplicationBuilder app)
  {
    if (Environment.GetEnvironmentVariable("xUnit") == "true") return;
    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
      var context = serviceScope.ServiceProvider.GetService<BookingContext>()!;
      if (context.Database.GetPendingMigrations().Any())
        context.Database.EnsureCreated();
    }
  }
}