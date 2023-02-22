using Hotel.Common.Data;
using Hotel.Search.Api.Contexts.Entities;

namespace Hotel.Search.Api.Rooms.Specifications;

public class ReservedRoomsSpecification : BaseSpecifcation<Reservation>
{
  public ReservedRoomsSpecification(DateTime firstDay, DateTime endDay)
  {
    Criteria = r => !r.Canceled && 
                    (
                      (r.StartAt >= firstDay && r.StartAt <= endDay) ||
                      (r.EndAt >= firstDay && r.EndAt <= endDay)
                    );
  }
}
