
using Microsoft.AspNetCore.Mvc;

using Homespirations.Core.DTOs;
using Homespirations.Core.Interfaces;
using Homespirations.Application.Services;


public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async ([FromBody] RegisterRequest request, AuthService authService) =>
       {
           var result = await authService.RegisterUserAsync(request);
           return result.IsSuccess ? Results.Ok("Successfully registered") : Results.BadRequest(result.Errors);
       });

        group.MapGet("/confirm-email", async (string userId, string token, IUserService userService) =>
        {
            var success = await userService.ConfirmEmailAsync(userId, token);
            return success ? Results.Ok("Email confirmed successfully!") : Results.BadRequest("Invalid token or user.");
        });
    }
}
