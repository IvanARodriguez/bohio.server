using System.Text.Json;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = new ErrorResponse("Server.Error", ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}

public class ErrorResponse
{
    public string Code { get; }
    public string Message { get; }

    public ErrorResponse(string code, string message)
    {
        Code = code;
        Message = message;
    }
}
