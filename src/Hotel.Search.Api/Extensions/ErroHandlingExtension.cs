using Hotel.Search.Api;
using System.Diagnostics.CodeAnalysis;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Diagnostics;

[ExcludeFromCodeCoverage]
public static class ErroHandlingExtension
{
  public static void UaseErrorHandling(this IApplicationBuilder app)
  {
    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
      AllowStatusCode404Response = true,
      ExceptionHandler = async (context) =>
      {
        var problemDetailsService = context.RequestServices.GetService<IProblemDetailsService>();
        if (problemDetailsService is null) return;

        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error;
        if (exception == null || exception.GetType() != typeof(SearchException))
          return;

        var title = "Bad Input";
        var detail = "Invalid input";
        var type = "https://errors.example.com/badInput";

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = Text.Plain;
        detail = exception.Message;

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
          HttpContext = context,
          ProblemDetails = { Type = type, Title = title, Detail = detail }
        });
      }
    });

  }
}