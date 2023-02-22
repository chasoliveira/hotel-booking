using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Api.Contexts;

[ExcludeFromCodeCoverage]
public class BookingContext : DbContext
{
  public BookingContext(DbContextOptions<BookingContext> options) : base(options)
  { }

  public DbSet<Entities.Room> Rooms { get; set; }
  public DbSet<Entities.Booking> Bookings { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Entities.Room>().HasData(new Entities.Room() { Id = -1, Description = "An awesome room for your vacation" });
    base.OnModelCreating(modelBuilder);
  }
}
