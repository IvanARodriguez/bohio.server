using System.Text.Json;
using Serilog;


namespace Homespirations.Api.Middlewares;

// Use zerolog
public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    readonly Serilog.ILogger _log = Log.ForContext<ExceptionHandlingMiddleware>();

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _log.Error(ex.Message);
            Console.WriteLine(ex.Message);
            var response = new ErrorResponse("Server.Error", "something went wrong");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}

public class ErrorResponse(string code, string message)
{
    public string Code { get; } = code;
    public string Message { get; } = message;
}
