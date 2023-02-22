using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.Results;

namespace Hotel.Search.Api.Rooms;

[ExcludeFromCodeCoverage]
public static class EndpointsV1
{
  public static RouteGroupBuilder MapRoomsApiV1(this RouteGroupBuilder group)
  {
    group
      .MapGet("/rooms", ListAvailableRoomsAsync)
      .WithDescription("Return a list of available rooms")
      .WithSummary("A list of available rooms")
      .Produces<List<RoomViewModel>>(HttpStatusCode.OK.GetHashCode())
      .Produces<ProblemDetails>(HttpStatusCode.BadRequest.GetHashCode())
      .WithOpenApi();

    return group;
  }

  public static async Task<IResult> ListAvailableRoomsAsync(
    IMediator mediator,
    IValidator<AvailableRoomsQuery> validator, 
    string startAt, string endAt)
  {
    var request = new AvailableRoomsQuery(startAt, endAt);
    var result = validator.Validate(request);
    if (!result.IsValid)
      return ValidationProblem(result.ToProblem());

    return TypedResults.Ok(await mediator.Send(request));
  }
}