using CreditService.Models;
using CreditService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessageHandling();
builder.Services.AddMessageHandler<OrderCreatedHandler>();
builder.Services.AddMessageHandler<OrderSucceededHandler>();
builder.Services.AddMessageHandler<OrderFailedHandler>();

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICreditRepository, CreditRepository>();
builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();

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