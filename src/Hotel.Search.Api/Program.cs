using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Hotel.Search.Api.Contexts;
using Hotel.Search.Api.Messaging;
using Hotel.Search.Api.Rooms;
using Serilog;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Logging.AddSerilog(logger);
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

builder.Services.AddMediatR(typeof(Hotel.Search.Api.Program));

builder.Services.AddValidatorsFromAssemblyContaining<Hotel.Search.Api.Program>();

builder.Services.AddCaching(builder.Configuration);

builder.Services.AddContextServices(builder.Configuration);

builder.Services.AddMessagingConsumer(builder.Configuration);

var app = builder.Build();

app.EnsureMigrations();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHealthChecks("/");

app.MapGroup("/api/rooms")
    .WithTags("Search.Api")
    .MapRoomsApiV1();

app.UaseErrorHandling();

app.Run();

namespace Hotel.Search.Api
{
  [ExcludeFromCodeCoverage]
  public partial class Program
  { }
}