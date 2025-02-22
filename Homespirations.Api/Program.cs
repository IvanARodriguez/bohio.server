using System.Text.Json;
using DotNetEnv;
using Homespirations.Api.Endpoints;
using Homespirations.Api.Middlewares;
using Homespirations.Application;
using Homespirations.Core.Entities;
using Homespirations.Core.Helpers;
using Homespirations.Infrastructure;
using Homespirations.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Add logging services
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
.ReadFrom.Configuration(context.Configuration)
.WriteTo.Console());

// Ensure configuration is available
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables(); // Load from environment as fallback

builder.Services.AddSerilog();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

Env.Load();

// Add services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


// providers
builder.Services.AddAntiforgery();
builder.Services.Configure<FormOptions>(options =>
   {
       options.MultipartBodyLengthLimit = 104857600; // Set a limit if needed (100MB in this example)
   });


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.Converters.Add(new UlidJsonConverter());
});

var app = builder.Build();
app.UseAntiforgery();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

var isTesting = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_TESTS") == "true";

if (!isTesting)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var databaseProvider = dbContext.Database.ProviderName;

        if (databaseProvider != "Microsoft.EntityFrameworkCore.InMemory")
        {
            dbContext.Database.OpenConnection();
            dbContext.Database.CloseConnection();
            Log.Information("Database connected successfully.");
        }
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Failed to connect to the database.");
    }
}

app.MapGet("/", () =>
{
    WelcomeMessage response = new()
    {
        Message = "Welcome to Homespirations!"
    };
    return Results.Json(response);
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapHomeSpaceEndpoints();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

Log.Information("App started in {environment} mode", builder.Environment.EnvironmentName);


app.Run();