using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Search.Api.Contexts;

[ExcludeFromCodeCoverage]
public class SearchContext : DbContext
{
  public SearchContext(DbContextOptions<SearchContext> options) : base(options)
  { }

  public DbSet<Entities.Room> Rooms { get; set; }
  public DbSet<Entities.Reservation> Reservations { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Entities.Room>().HasData(new Entities.Room() { Id = -1, ReferenceId = 1, Description = "An awesome room for your vacation" });
    base.OnModelCreating(modelBuilder);
  }
}
