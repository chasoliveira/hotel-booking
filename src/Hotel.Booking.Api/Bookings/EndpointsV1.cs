using System.Diagnostics.CodeAnalysis;
using System.Net;
using Hotel.Booking.Api.Bookings.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Booking.Api.Bookings;
[ExcludeFromCodeCoverage]
public static class EndpointsV1
{
  public static RouteGroupBuilder MapBookingsApiV1(this RouteGroupBuilder group)
  {
    group
      .MapGet("/{id}", GetBookingAsync)
      .WithDescription("Return the booking by a given ID")
      .WithSummary("A detailing booking for a given ID")
      .Produces<BookingViewModel>(HttpStatusCode.OK.GetHashCode())
      .Produces<ProblemDetails>(HttpStatusCode.BadRequest.GetHashCode())
      .AddEndpointFilter<AfterEndpointExecution>();

    group
      .MapGet("/", ListBookingAsync)
      .WithDescription("Return a list of bookings by a given user name")
      .WithSummary("A list with detailing bookings for a given user name")
      .Produces<List<BookingViewModel>>(HttpStatusCode.OK.GetHashCode())
      .Produces<ProblemDetails>(HttpStatusCode.BadRequest.GetHashCode())
      .AddEndpointFilter<AfterEndpointExecution>();

    group
      .MapPost("/", CreateBookingAsync)
      .WithDescription("Create a new booking for a given user name, and according to business roles")
      .WithSummary("Create a new booking")
      .Produces(HttpStatusCode.NoContent.GetHashCode())
      .Produces<ProblemDetails>(HttpStatusCode.BadRequest.GetHashCode())
      .AddEndpointFilter<AfterEndpointExecution>();

    group
      .MapPut("/{id}", PerformActionAsync<UpdateBookingCommand>)
      .WithDescription("Update an existing booking by a given ID")
      .WithSummary("Update an existing booking by a given ID")
      .Produces<BookingViewModel>(HttpStatusCode.OK.GetHashCode())
      .Produces<ProblemDetails>(HttpStatusCode.BadRequest.GetHashCode())
      .AddEndpointFilter<AfterEndpointExecution>();

    group
      .MapPatch("/{id}/cancel", PerformActionAsync<CancelBookingCommand>)
      .WithDescription("Update an existing booking to be canceled by a given ID")
      .WithSummary("Cancel an existing booking by a given ID")
      .Produces<BookingViewModel>(HttpStatusCode.OK.GetHashCode())
      .Produces<ProblemDetails>(HttpStatusCode.BadRequest.GetHashCode())
      .AddEndpointFilter<AfterEndpointExecution>();

    group
      .MapPatch("/{id}/confirm", PerformActionAsync<ConfirmBookingCommand>)
      .WithDescription("Update an existing booking to be confirmed by a given ID")
      .WithSummary("Confirm an existing booking by a given ID")
      .Produces<BookingViewModel>(HttpStatusCode.OK.GetHashCode())
      .Produces<ProblemDetails>(HttpStatusCode.BadRequest.GetHashCode())
      .AddEndpointFilter<AfterEndpointExecution>();

    group
      .MapDelete("/{id}", DeleteAsync)
      .WithDescription("Delete an existing booking by a given ID")
      .WithSummary("Delete an existing booking by a given ID")
      .Produces<BookingViewModel>(HttpStatusCode.OK.GetHashCode())
      .Produces<ProblemDetails>(HttpStatusCode.BadRequest.GetHashCode())
      .AddEndpointFilter<AfterEndpointExecution>();

    return group.WithOpenApi();
  }

  public static async Task<IResult> GetBookingAsync(IMediator mediator, int id, string userName)
  {
    var response = await mediator.Send(new BookingByIdQuery(id, userName));

    if (response is null) return TypedResults.NotFound();

    return TypedResults.Ok(response);
  }
  private static async Task<IResult> ListBookingAsync(IMediator mediator, string userName)
  {
    var response = await mediator.Send(new MyBookingsQuery(userName));

    if (response is null) return TypedResults.NotFound();

    return TypedResults.Ok(response);
  }

  private static async Task<IResult> CreateBookingAsync(IMediator mediator, [FromBody] NewBookingCommand command)
  {
    var (success, id) = await mediator.Send(command);

    if (!success)
      return TypedResults.BadRequest("No booking was created, please confirm and try again.");

    return TypedResults.Created($"/api/bookings/{id}");
  }
  private static Task<IResult> DeleteAsync(IMediator mediator, int id, string userName)
    => PerformActionAsync(mediator, id, new DeleteBookingCommand(id, userName));
  

  private static async Task<IResult> PerformActionAsync<TCommand>(IMediator mediator, int id, [FromBody] TCommand command) where TCommand : IRequest<bool>
  {
    var success = await mediator.Send(command);

    if (!success)
      return TypedResults.BadRequest($"The action for booking Id '{id}' was not be performed, please confirm and try again.");

    return TypedResults.NoContent();
  }
}