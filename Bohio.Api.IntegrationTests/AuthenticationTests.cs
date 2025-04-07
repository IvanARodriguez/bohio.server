using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Bohio.Core.DTOs;
using Bohio.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUlid;

namespace Bohio.Api.IntegrationTests;

public partial class AuthenticationTests : IClassFixture<BohioWebAppFactory>
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<AuthenticationTests> _logger;
  public AuthenticationTests()
  {
    BohioWebAppFactory app = new();
    _httpClient = app.CreateClient(new WebApplicationFactoryClientOptions
    {
      AllowAutoRedirect = false
    });
    var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
    _logger = loggerFactory.CreateLogger<AuthenticationTests>();

    MockEmailService.SentEmails.Clear();
  }

  [Fact]
  public async Task Can_Register_Confirm_And_Login()
  {
    // Register
    var registerResponse = await _httpClient.PostAsJsonAsync("/api/auth/register", new RegisterRequest
    {
      Email = "ivan@hitab.dev",
      Password = "SecureP@ssw0rd1991!",
      FirstName = "Test",
      LastName = "Tester",
    });
    registerResponse.EnsureSuccessStatusCode();
    _logger.LogInformation("Register response: {StatusCode}", registerResponse.StatusCode);
    var email = MockEmailService.SentEmails.FirstOrDefault();
    Assert.NotNull(email);
    var (userId, token) = ExtractUserIdAndToken(email.HtmlBody);

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
    {
      throw new Exception("user ID or Token are empty when registering the user");
    }
    if (!Ulid.TryParse(userId, out var userUlid))
    {
      _logger.LogError("User ID or Token are empty when registering the user");
      throw new Exception("could not parse user id to Ulid");
    }
    _logger.LogInformation("User ID: {UserId}, Token: {Token}", userId, token);
    // Confirm
    var confirmResponse = await _httpClient.PostAsJsonAsync("/api/auth/confirm-email", new ConfirmEmailRequest
    {
      UserId = userUlid,
      Token = token
    });

    _logger.LogInformation("ConfirmEmail response: {StatusCode}", confirmResponse.StatusCode);

    var confirmContent = await confirmResponse.Content.ReadAsStringAsync();
    _logger.LogInformation("ConfirmEmail response body: {Content}", confirmContent);

    confirmResponse.EnsureSuccessStatusCode();

    // Login
    var loginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new LoginRequest
    {
      Email = "ivan@hitab.dev",
      Password = "SecureP@ssw0rd1991!"
    });

    loginResponse.EnsureSuccessStatusCode();
    var cookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
    Assert.Contains(cookies, c => c.StartsWith("AccessToken"));
    Assert.Contains(cookies, c => c.StartsWith("RefreshToken"));
  }

  private (string userId, string token) ExtractUserIdAndToken(string body)
  {
    var userIdMatch = UserIdRegex().Match(body);
    var tokenMatch = TokenRegex().Match(body);

    if (!userIdMatch.Success || !tokenMatch.Success)
    {
      throw new InvalidOperationException("Could not extract userId and token from email.");
    }

    var userId = userIdMatch.Groups[1].Value;
    var token = tokenMatch.Groups[1].Value.Trim('"');

    return (userId, token);
  }


  [GeneratedRegex(@"userId=([^&\s]+)")]
  private static partial Regex UserIdRegex();

  [GeneratedRegex(@"token=([^&\s]+)")]
  private static partial Regex TokenRegex();
}
