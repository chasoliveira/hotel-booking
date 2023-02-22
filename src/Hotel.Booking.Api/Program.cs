using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Hotel.Booking.Api;
using Hotel.Booking.Api.Bookings;
using Hotel.Booking.Api.Contexts;
using Hotel.Common.Messaging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Logging.AddSerilog(logger);
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

builder.Services.AddMediatR(typeof(Hotel.Booking.Api.Program));

builder.Services.AddContextServices(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<Hotel.Booking.Api.Program>();

builder.Services.AddCaching(builder.Configuration);

builder.Services.AddMessaging(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/");

app.MapGroup("/api/bookings")
.MapBookingsApiV1()
.WithTags("Booking.Api");

app.Run();

namespace Hotel.Booking.Api
{
  [ExcludeFromCodeCoverage]
  public partial class Program
  { }
}
