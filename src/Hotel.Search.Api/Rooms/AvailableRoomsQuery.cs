using MediatR;

namespace Hotel.Search.Api.Rooms;

public record AvailableRoomsQuery : IRequest<IEnumerable<RoomViewModel>>
{
  public AvailableRoomsQuery(string startAt, string endAt)
  {

    if (!DateTime.TryParse(startAt, out DateTime startAtParsed))
      throw new SearchException($" The parameter '{nameof(startAt)}' has an invalid value: '{startAt}'");

    if (!DateTime.TryParse(endAt, out DateTime endAtParsed))
      throw new SearchException($" The parameter '{nameof(endAt)}' has an invalid value: '{endAt}'");

    StartAt = startAtParsed;
    EndAt = endAtParsed;
  }

  public const string CacheKey = nameof(AvailableRoomsQuery);
  public DateTime StartAt { get; }
  public DateTime EndAt { get; }
  public List<DateTime> NextThirtyDays
    => Enumerable.Range(0, 30)
                  .Select(day => DateTime.Now.AddDays(day).Date)
                  .ToList();
  public DateTime FirstDay => NextThirtyDays.First();
  public DateTime LastDay => NextThirtyDays.Last().AddDays(1).AddSeconds(-1);

}
