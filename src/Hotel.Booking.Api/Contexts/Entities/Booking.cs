using System.Diagnostics.CodeAnalysis;

namespace Hotel.Booking.Api.Contexts.Entities;

[ExcludeFromCodeCoverage]
public class Booking
{
  public int Id { get; set; }
  public int RoomId { get; set; }
  public string UserName { get; set; }
  public virtual Room Room {get;set;}
  public DateTime StartAt { get; set; }
  public DateTime EndAt { get; set; }
  public bool IsCanceled { get; set; }
  public bool IsConfirmed { get; set; }
}