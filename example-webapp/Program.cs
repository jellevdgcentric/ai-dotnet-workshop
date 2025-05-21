using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Russkyc.MinimalApi.Framework;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

// Make sure these lines exist in the services configuration section
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var assembly = Assembly.GetExecutingAssembly();

// Add Entity Context Services
builder.Services.AddDbContextService(assembly, options => options.UseInMemoryDatabase("sample"));

// Uncomment to enable realtime events
// by default the endpoint used is "/crud-events"
// this can be changed by providing a string parameter, eg; `MapRealtimeHub("/api-events")`
builder.Services.AddRealtimeService();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.UseHttpsRedirection();

// Map Entity CRUD Endpoints
app.MapGroup("api")
    .MapAllEntityEndpoints<int>(assembly);

// Map Realtime Hub
app.MapRealtimeHub();

app.Run();
