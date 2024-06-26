using BusinessLogic.Managers;
using Microsoft.Extensions.Configuration;
using PracticaCerti2.Middlewares;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile(
        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json"
    )
    .Build();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<PatientManager>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandlerMiddleware();

if (app.Environment.EnvironmentName == "QA")
{
    Log.Logger = new LoggerConfiguration()
    .WriteTo.File(app.Configuration.GetSection("Paths").GetSection("logs").Value, rollingInterval: RollingInterval.Day)
    .CreateLogger();
    Log.Information("Initializing the server!!");
}
else
{
    Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(app.Configuration.GetSection("Paths").GetSection("logs").Value, rollingInterval: RollingInterval.Day)
    .CreateLogger();
    Log.Information("Initializing the server!!");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "QA" || app.Environment.EnvironmentName == "UAT")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.DocumentTitle = app.Configuration.GetSection("ApplicationSettings").GetSection("ApplicationName").Value
    );
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
