using ConcurrencyDemo.Application;
using ConcurrencyDemo.Application.Services;
using ConcurrencyDemo.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using var scope = app.Services.CreateScope();
var demo = scope.ServiceProvider.GetRequiredService<IConcurrencyDemoService>();
await demo.RunAsync();
