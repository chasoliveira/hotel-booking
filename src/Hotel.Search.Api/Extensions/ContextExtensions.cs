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
}