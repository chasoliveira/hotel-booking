using System.Diagnostics.CodeAnalysis;

namespace Hotel.Search.Api.Contexts.Entities;

[ExcludeFromCodeCoverage]
public class Reservation
{
  public int Id { get; set; }
  public int ReferenceId { get; set; }
  public int RoomId { get; set; }
  public int ReferenceRoomId { get; set; }
  public string UserName { get; set; }
  public virtual Room Room {get;set;}
  public DateTime StartAt { get; set; }
  public DateTime EndAt { get; set; }
  public bool Canceled { get; set; }
  public bool Confirmed { get; set; }
}