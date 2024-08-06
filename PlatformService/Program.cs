using MassTransit;
using MassTransit.Futures.Contracts;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Data.Repos;
using PlatformService.Data.Repos.Caching;
using PlatformService.Services.RabbitMq_MassTransit;
using StackExchange.Redis;
using static MassTransit.Logging.DiagnosticHeaders.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
builder.Services.AddScoped<IPlatformRepo,PlatformRepo>();
builder.Services.AddScoped<ICacheService,ICacheService>();
builder.Services.AddScoped<IDriverNotificationPublisherService, DriverNotificationPublisherService>();

var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitMqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitMqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";

// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
	x.UsingRabbitMq((context, cfg) =>
	{
		cfg.Host(rabbitMqHost, rabbitMqPort, "/", h =>
		{
			h.Username(rabbitMqUser);
			h.Password(rabbitMqPassword);
		});

		// Configure endpoints if needed
		cfg.ConfigureEndpoints(context);
	});
});



// Add controllers
builder.Services.AddControllers();

// Register IBus
builder.Services.AddMassTransitHostedService();

builder.Services.AddControllers();
               

// Other service registrations
builder.Services.AddControllers();
             
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
