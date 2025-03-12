
using Microsoft.AspNetCore.Mvc;

using Bohio.Core.DTOs;
using Bohio.Core.Interfaces;
using Bohio.Application.Services;
using Bohio.Core.Types;


public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (HttpContext ctx, [FromBody] RegisterRequest request, AuthService authService) =>
       {
           var path = ctx.Request.Path.Value;
           Language lang = Language.EN;
           if (path != null && path.StartsWith("/es", StringComparison.OrdinalIgnoreCase))
           {
               lang = Language.ES;
           }
           var result = await authService.RegisterUserAsync(request, lang);
           return result.IsSuccess ? Results.Ok("Successfully registered") : Results.BadRequest(result.Errors);
       });

        group.MapGet("/confirm-email", async (string userId, string token, IUserService userService) =>
        {
            var success = await userService.ConfirmEmailAsync(userId, token);
            return success ? Results.Ok("Email confirmed successfully!") : Results.BadRequest("Invalid token or user.");
        });
    }
}
