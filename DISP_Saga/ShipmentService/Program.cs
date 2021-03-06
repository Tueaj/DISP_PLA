using MessageHandling;
using Microsoft.AspNetCore.Builder;
using ShipmentService;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddMessageHandler<ShipOrderHandler>();
services.AddMessageHandling();

var app = builder.Build();

app.Run();