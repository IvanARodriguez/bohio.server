using System.Text.Json;
using Amazon.S3;
using DotNetEnv;
using FluentValidation;
using Homespirations.Api.Endpoints;
using Homespirations.Api.Middlewares;
using Homespirations.Application.Services;
using Homespirations.Application.Validators;
using Homespirations.Core.Entities;
using Homespirations.Core.Helpers;
using Homespirations.Core.Interfaces;
using Homespirations.Infrastructure.Repositories;
using Homespirations.Infrastructure.Services;
using Homespirations.Infrastructure.Storage;
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

builder.Services.AddSerilog();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

Env.Load();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    Log.Fatal("Database connection string is missing.");
    throw new Exception("Database connection string is missing.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.EnableSensitiveDataLogging();
    options.UseNpgsql(connectionString);
});

builder.Services.AddAntiforgery();

builder.Services.Configure<FormOptions>(options =>
   {
       options.MultipartBodyLengthLimit = 104857600; // Set a limit if needed (100MB in this example)
   });

// Dependency injections
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<HomeSpaceService>();
builder.Services.AddScoped<MediaServices>();
builder.Services.AddScoped<IImageOptimizer, ImageOptimizer>();
builder.Services.AddScoped<ICloudStorage, CloudStorage>();
builder.Services.AddSingleton<IValidator<HomeSpace>, HomeSpaceValidator>();
builder.Services.AddSingleton<IValidator<Media>, MediaValidator>();
builder.Services.AddSingleton<IValidator<FormFile>, FormFileValidator>();
builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.Converters.Add(new UlidJsonConverter());
});

var app = builder.Build();
app.UseAntiforgery();
app.UseSerilogRequestLogging();

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