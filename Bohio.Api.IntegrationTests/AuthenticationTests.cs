using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Bohio.Core.DTOs;
using Bohio.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using NUlid;

namespace Bohio.Api.IntegrationTests;

public partial class AuthenticationTests
{
  private readonly HttpClient _httpClient;
  public AuthenticationTests()
  {
    BohioWebAppFactory app = new();
    _httpClient = app.CreateClient(new WebApplicationFactoryClientOptions
    {
      AllowAutoRedirect = false
    });
    MockEmailService.SentEmails.Clear();
  }

  [Fact]
  public async Task Can_Register_Confirm_And_Login()
  {
    // Register
    var registerResponse = await _httpClient.PostAsJsonAsync("/api/auth/register", new RegisterRequest
    {
      Email = "ivan@hitab.dev",
      Password = "SecureP@ssw0rd!"

    });
    registerResponse.EnsureSuccessStatusCode();
    var email = MockEmailService.SentEmails.FirstOrDefault();
    Assert.NotNull(email);
    var (userId, token) = ExtractUserIdAndToken(email.HtmlBody);

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
    {
      throw new Exception("user ID or Token are empty when registering the user");
    }

    if (Ulid.TryParse(userId, out var userUlid))
    {
      throw new Exception("could not parse user id to Ulid");
    }

    // Confirm
    var confirmResponse = await _httpClient.PostAsJsonAsync("/api/auth/confirm-email", new ConfirmEmailRequest
    {
      UserId = userUlid,
      Token = token
    });

    confirmResponse.EnsureSuccessStatusCode();

    // Login
    var loginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new LoginRequest
    {
      Email = "ivan@hitab.dev",
      Password = "SecureP@ssw0rd!"
    });

    loginResponse.EnsureSuccessStatusCode();
    var cookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
    Assert.Contains(cookies, c => c.StartsWith("access-token"));
    Assert.Contains(cookies, c => c.StartsWith("refresh-token"));
  }

  private (string userId, string token) ExtractUserIdAndToken(string body)
  {
    var userIdMatch = UserIdRegex().Match(body);
    var tokenMatch = TokenRegex().Match(body);

    if (!userIdMatch.Success || !tokenMatch.Success)
    {
      throw new InvalidOperationException("Could not extract userId and token from email.");
    }

    return (userIdMatch.Groups[1].Value, tokenMatch.Groups[1].Value);
  }

  [GeneratedRegex(@"userId=([^&\s]+)")]
  private static partial Regex UserIdRegex();

  [GeneratedRegex(@"token=([^&\s]+)")]
  private static partial Regex TokenRegex();
}
