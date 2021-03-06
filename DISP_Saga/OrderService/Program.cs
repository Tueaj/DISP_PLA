using System.Text.Json.Serialization;
using MessageHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Models;
using OrderService.Services;
using OrderService.Services.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessageHandling();
builder.Services.AddMessageHandler<CreditRequestAckHandler>();
builder.Services.AddMessageHandler<InventoryRequestAckHandler>();
builder.Services.AddMessageHandler<ShipOrderAckHandler>();
builder.Services.AddMessageHandler<CommitCreditAckHandler>();
builder.Services.AddMessageHandler<CommitInventoryAckHandler>();
builder.Services.AddMessageHandler<CommitCreditNackHandler>();
builder.Services.AddMessageHandler<CommitInventoryNackHandler>();
builder.Services.AddMessageHandler<CreditRequestNackHandler>();
builder.Services.AddMessageHandler<RollbackInventoryAckHandler>();
builder.Services.AddMessageHandler<RollbackCreditAckHandler>();
builder.Services.AddMessageHandler<InventoryRequestNackHandler>();
builder.Services.AddMessageHandler<AbortInventoryAckHandler>();
builder.Services.AddMessageHandler<AbortCreditAckHandler>();

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
        
        var enumConverter = new JsonStringEnumConverter();
        configure.JsonSerializerOptions.Converters.Add(enumConverter);
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<OrderStatusService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();