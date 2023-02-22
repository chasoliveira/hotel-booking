using System.Diagnostics.CodeAnalysis;

namespace Hotel.Search.Api.Contexts.Entities;

[ExcludeFromCodeCoverage]
public class Room
{
  public int Id { get; set; }
  public int ReferenceId { get; set; }
  public string Description { get; set; }
  public ICollection<Reservation> Reservations { get; set; } = Array.Empty<Reservation>();
}