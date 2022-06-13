using System.Text.Json.Serialization;
using CreditService.Models;
using CreditService.Repository;
using CreditService.Services;
using MessageHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessageHandling();
builder.Services.AddMessageHandler<CreditRequestHandler>();
builder.Services.AddMessageHandler<CommitCreditHandler>();
builder.Services.AddMessageHandler<AbortCreditHandler>();
builder.Services.AddMessageHandler<RollbackCreditHandler>();

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
builder.Services.AddSingleton<ICreditRepository, CreditRepository>();
builder.Services.AddHostedService<TimeoutDetectorService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();