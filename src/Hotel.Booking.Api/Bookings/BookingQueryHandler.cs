using Hotel.Booking.Api.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Api.Bookings;

public record BookingViewModel(int Id, string Room, string UserName, DateTime StartAt, DateTime EndAt, bool IsCanceled, bool IsConfirmed)
{
  public static implicit operator BookingViewModel(Api.Contexts.Entities.Booking model)
    => new BookingViewModel(model.Id, model.Room.Description, model.UserName, model.StartAt, model.EndAt, model.IsCanceled, model.IsConfirmed);
}

public record BookingByIdQuery(int Id, string UserName) : IRequest<BookingViewModel>;
public record MyBookingsQuery(string UserName) : IRequest<IReadOnlyList<BookingViewModel>>;

public class BookingQueryHandler :
 IRequestHandler<BookingByIdQuery, BookingViewModel>,
 IRequestHandler<MyBookingsQuery, IReadOnlyList<BookingViewModel>>
{
  private readonly BookingContext context;

  public BookingQueryHandler(BookingContext context) => this.context = context;

  public async Task<BookingViewModel> Handle(BookingByIdQuery request, CancellationToken cancellationToken)
    => (await context.Bookings
                     .BaseQuery()
                     .WhereBy(request)
                     .AsViewModel()
                     .FirstOrDefaultAsync())!;

  public async Task<IReadOnlyList<BookingViewModel>> Handle(MyBookingsQuery request, CancellationToken cancellationToken)
    => await context.Bookings
                    .BaseQuery()
                    .WhereBy(request)
                    .AsViewModel()
                    .ToListAsync();
}

public static class BookingQueryHandlerExtensions
{
  public static IQueryable<Contexts.Entities.Booking> WhereBy(this IQueryable<Contexts.Entities.Booking> bookings, BookingByIdQuery queryBy)
    => bookings.Where(b => b.Id == queryBy.Id);

  public static IQueryable<Contexts.Entities.Booking> WhereBy(this IQueryable<Contexts.Entities.Booking> bookings, MyBookingsQuery queryBy)
    => bookings.Where(b => b.UserName == queryBy.UserName);

  public static IQueryable<Contexts.Entities.Booking> BaseQuery(this IQueryable<Contexts.Entities.Booking> bookings)
    => bookings.Include(b => b.Room).AsNoTracking();

  public static IQueryable<BookingViewModel> AsViewModel(this IQueryable<Contexts.Entities.Booking> bookings)
    => bookings.Select(b => new BookingViewModel(
                            b.Id,
                            b.Room.Description,
                            b.UserName,
                            b.StartAt,
                            b.EndAt,
                            b.IsCanceled,
                            b.IsConfirmed));
}