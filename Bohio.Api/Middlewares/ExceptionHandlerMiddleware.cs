using System.Net;
using System.Text.Json;
using FluentValidation;
using Serilog;

namespace Bohio.Api.Middlewares;

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
    catch (ValidationException validationEx) // Handle FluentValidation errors
    {
      _log.Warning("Validation error: {Errors}", validationEx.Errors);

      var errors = validationEx.Errors
          .Select(e => new ErrorResponse(e.ErrorCode, e.ErrorMessage))
          .ToList();

      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      await context.Response.WriteAsync(JsonSerializer.Serialize(errors));
    }
    catch (Exception ex)
    {
      _log.Error(ex, "Unhandled exception");

      var response = new ErrorResponse("Server.Error", ex.Message); // Show the actual error message

      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
  }
}

public class ErrorResponse(string code, string message)
{
  public string Code { get; } = code;
  public string Message { get; } = message;
}
