using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Bohio.Core.DTOs;
using Bohio.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using NUlid;

namespace Bohio.Api.IntegrationTests;

internal static partial class AuthTestHelper
{
  internal static async Task<HttpClient> GetAuthenticatedClient(BohioWebAppFactory appFactory)
  {
    var email = $"testUser_{Guid.NewGuid():N}@test.com";
    var password = "SecuredP@ssw0rd1991";
    var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
    {
      AllowAutoRedirect = false
    });

    var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
    {
      Email = email,
      Password = password,
      FirstName = "Test",
      LastName = "User"
    });

    registerResponse.EnsureSuccessStatusCode();

    var sentEmail = MockEmailService.SentEmails.FirstOrDefault(e => e.To == email) ?? throw new Exception("Confirmation email not found");

    var (userId, token) = ExtractUserIdAndToken(sentEmail.HtmlBody);

    var confirmResponse = await client.PostAsJsonAsync("/api/auth/confirm-email", new ConfirmEmailRequest
    {
      UserId = Ulid.Parse(userId),
      Token = token
    });

    confirmResponse.EnsureSuccessStatusCode();

    var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
    {
      Email = email,
      Password = password
    });

    if (loginResponse.Headers.TryGetValues("Set-Cookie", out var setCookie))
    {
      foreach (var cookie in setCookie)
      {
        client.DefaultRequestHeaders.Add("Cookie", cookie);
      }
    }

    return client;
  }

  private static (string userId, string token) ExtractUserIdAndToken(string htmlBody)
  {
    var userIdMatch = UserIdRegex().Match(htmlBody);
    var tokenMatch = TokenRegex().Match(htmlBody);
    return (userIdMatch.Groups[1].Value, tokenMatch.Groups[1].Value.Trim('"'));
  }

  [GeneratedRegex(@"userId=([^&\s]+)")]
  private static partial Regex UserIdRegex();
  [GeneratedRegex(@"token=([^&\s]+)")]
  private static partial Regex TokenRegex();
}
