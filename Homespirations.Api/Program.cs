using System.Text.Json;
using DotNetEnv;
using Homespirations.Api.Endpoints;
using Homespirations.Api.Middlewares;
using Homespirations.Application.Services;
using Homespirations.Core.Helpers;
using Homespirations.Core.Interfaces;
using Homespirations.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Add logging services
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Console());

    builder.Services.AddSerilog();


    Env.Load();

    string? connectionString = Environment.GetEnvironmentVariable("DefaultConnection");


    if (string.IsNullOrEmpty(connectionString))
    {
        Log.Fatal("Database connection string is missing.");
        throw new Exception("Database connection string is missing.");
    }

    builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<HomeSpaceService>();

    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new UlidJsonConverter());
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.OpenConnection();
        dbContext.Database.CloseConnection();
        Log.Information("Database connected successfully.");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Failed to connect to the database.");
    }

    app.MapGet("/", () =>
 {
     Log.Information("Hello, world!");
     SuccessResponse response = new();
     response.Message = "Welcome to Homespirations!";
     return Results.Json(response);
 });

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.MapHomeSpaceEndpoints();


    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    Log.Information("App started in {environment} mode", builder.Environment.EnvironmentName);


    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


public class SuccessResponse { public string Message { get; set; } = "Success"; }