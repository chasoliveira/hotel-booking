using Hotel.Common.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Hotel.Search.Api.Contexts;
using Hotel.Search.Api.Rooms.Specifications;
using Hotel.Common;

namespace Hotel.Search.Api.Rooms;

public record AvailableDayViewModel(DateTime Date);
public record RoomViewModel(int Id, string Description, IEnumerable<AvailableDayViewModel> AvailableDays);

internal record GroupedRooms(int Id, string Description);
public record ReservedProjection(int Id, string Description, DateTime StartAt, DateTime EndAt)
{
  public int TotalDays => (int)(EndAt.Date - StartAt.Date).TotalDays;
};

public class AvailableRoomsHandler : IRequestHandler<AvailableRoomsQuery, IEnumerable<RoomViewModel>>
{
  private readonly ICacheManagement _cache;
  private readonly SearchContext _context;

  public AvailableRoomsHandler(ICacheManagement cache, SearchContext context)
  {
    _cache = cache;
    _context = context;
  }

  public async Task<IEnumerable<RoomViewModel>> Handle(AvailableRoomsQuery request, CancellationToken cancelToken)
  {
    var cachedResult = await _cache.GetAsync<List<ReservedProjection>>(AvailableRoomsQuery.CacheKey, cancelToken);
    if (cachedResult != null)
      return GetGroupedResult(request, cachedResult);

    var queryResult = await _context.Reservations.Include(r => r.Room)
                     .Specify(new ReservedRoomsSpecification(request.StartAt, request.EndAt))
                     .Select(q => new ReservedProjection(q.RoomId, q.Room.Description, q.StartAt, q.EndAt))
                     .ToListAsync();

    var groupedResult = GetGroupedResult(request, queryResult);
    
    if (!queryResult.Any())
      groupedResult = await GetRoomsAsync(request);

    await CacheAndReturnAsync(groupedResult, cancelToken);

    return groupedResult!;
  }

  private async Task CacheAndReturnAsync(IEnumerable<RoomViewModel> groupedResult, CancellationToken cancelToken)
  {
    if (groupedResult != null && groupedResult.Any())
      await _cache.SetAsync(AvailableRoomsQuery.CacheKey, groupedResult, cancelToken);
  }

  private async Task<IEnumerable<RoomViewModel>> GetRoomsAsync(AvailableRoomsQuery request)
  {
    var rooms = await _context.Rooms.ToListAsync();
    return rooms.GroupBy(g => new GroupedRooms(g.Id, g.Description))
    .Select(s => new RoomViewModel(s.Key.Id, s.Key.Description, request.NextThirtyDays.Where(d => d <= request.EndAt)
                              .Select(d => new AvailableDayViewModel(d))));
  }

  private static IEnumerable<RoomViewModel> GetGroupedResult(AvailableRoomsQuery request, List<ReservedProjection> query)
  {
    var grouped = query.GroupBy(g => new GroupedRooms(g.Id, g.Description))
                 .Select(s => new RoomViewModel(s.Key.Id, s.Key.Description, GetAvaiablesDays(request, s)))
                 .ToList();

    return grouped;
  }

  private static IEnumerable<AvailableDayViewModel> GetAvaiablesDays(
     AvailableRoomsQuery request,
     IGrouping<GroupedRooms, ReservedProjection> group)
  {
    var expandedDays = group
        .SelectMany(sg => Enumerable.Range(0, sg.TotalDays)
        .Select(days => sg.StartAt.AddDays(days).Date));

    return request.NextThirtyDays.Where(d => d <= request.EndAt).Except(expandedDays)
                                .Select(d => new AvailableDayViewModel(d));
  }
}