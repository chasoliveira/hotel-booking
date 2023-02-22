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
}