using System.Diagnostics.CodeAnalysis;

namespace Hotel.Booking.Api.Contexts.Entities;

[ExcludeFromCodeCoverage]
public class Room
{
  public int Id { get; set; }
  public string Description { get; set; }
  public ICollection<Booking> Reservations { get; set; } = new List<Booking>();
}