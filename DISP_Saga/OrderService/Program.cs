using MessageHandling;
using Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Models;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessageHandling();
builder.Services.AddMessageHandler<CreditRequestAckHandler>();

builder.Services.AddOptions<MongoConnectionSettings>()
    .Configure<IConfiguration>((options, configuration) =>
    {
        configuration.GetSection(MongoConnectionSettings.Key).Bind(options);
    });

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(configure =>
    {
        configure.JsonSerializerOptions.IncludeFields = true;
    });

builder.Services.AddScoped<OrderLogic>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();