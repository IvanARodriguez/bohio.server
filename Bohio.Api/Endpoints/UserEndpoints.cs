
using Microsoft.AspNetCore.Mvc;

using Bohio.Core.DTOs;
using Bohio.Application.Services;
using Bohio.Core.Types;

namespace Bohio.Api.Endpoints;
public static class AuthEndpoints
{
  public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/auth");

    group.MapPost("/register", async (HttpContext ctx, [FromBody] RegisterRequest request, AuthService authService) =>
    {
      var supportedLanguages = new HashSet<string> { "en", "es" };
      Language lang = Language.EN;

      var referer = ctx.Request.Headers.Referer.ToString();
      if (!string.IsNullOrEmpty(referer))
      {
        var refererUri = new Uri(referer);
        var refererSegments = refererUri.AbsolutePath.Trim('/').Split('/');
        if (refererSegments.Length > 0 && supportedLanguages.Contains(refererSegments[0]))
        {
          lang = Enum.TryParse<Language>(refererSegments[0], true, out var detectedLang) ? detectedLang : Language.EN;
        }
      }

      var result = await authService.RegisterUserAsync(request, lang);
      return result.IsSuccess ? Results.Ok("Successfully registered") : Results.BadRequest(result.Errors);
    });

    group.MapPost("/confirm-email", async ([FromBody] ConfirmEmailRequest request, AuthService authService) =>
    {

      var result = await authService.ConfirmEmailAsync(request);
      return result.IsSuccess ? Results.Ok("Email confirmed successfully!") : Results.BadRequest(result.Errors);
    });

    group.MapPost("/login", async ([FromBody] LoginRequest request, AuthService authService) =>
    {
      var result = await authService.LoginAsync(request);
      return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Errors);
    });

    group.MapGet("/refresh", async (HttpRequest request, AuthService authService) =>
    {
      var refreshToken = request.Cookies["RefreshToken"];
      if (refreshToken == null)
      {
        return Results.Unauthorized();
      }
      var result = await authService.RefreshTokenAsync(refreshToken);
      return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Errors);
    });

    group.MapGet("/logout", (HttpRequest request, AuthService authService) =>
    {
      var result = authService.Logout();
      return result.IsSuccess
              ? Results.Ok(new { message = "Successfully logged out" })
              : Results.BadRequest(result.Errors);
    });

    group.MapGet("/me", async (HttpRequest request, AuthService authService) =>
    {
      var refreshToken = request.Cookies["RefreshToken"];
      var accessToken = request.Cookies["AccessToken"];
      if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
      {
        return Results.Unauthorized();
      }
      var result = await authService.GetUserAsync(accessToken);

      return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
    });
  }
}
