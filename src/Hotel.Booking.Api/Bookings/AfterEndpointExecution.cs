using System.Net;
using System.Text.Json;
using Hotel.Common.Notifications;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Hotel.Booking.Api.Bookings;


public class AfterEndpointExecution : IEndpointFilter
{
  private readonly INotificationContext notification;
  public AfterEndpointExecution(INotificationContext notification)
  {
    this.notification = notification;
  }

  public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
  {
    var result = await next(context);

    if ((result is BadRequest<string> dp) && notification.Any())
    {
      var problemDetails = new ProblemDetails
      {
        Status = HttpStatusCode.BadRequest.GetHashCode(),
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        Title = dp.Value,
        Detail = JsonSerializer.Serialize(notification.ReadOnlyList().Select(n => n.Message).ToList()),
        Instance = context.HttpContext.Request.Path
      };

      return Results.Problem(problemDetails);
    }
    return result;
  }
}
