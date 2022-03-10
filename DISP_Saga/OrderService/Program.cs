using CreditService.Models;
using MessageHandling;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessageHandling();

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();