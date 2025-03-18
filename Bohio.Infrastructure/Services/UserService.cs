using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Bohio.Core.DTOs;
using Bohio.Core.Results;
using Bohio.Core.Interfaces;
using Bohio.Infrastructure.Identity;
using Bohio.Core.Entities;
using Bohio.Core.Types;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using NUlid;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Bohio.Infrastructure.Services;

public class UserService(
    UserManager<AppUser> userManager,
    IMapper mapper,
    ILogger<UserService> logger,
    IEmailService emailService,
    IHttpContextAccessor httpContextAccessor
) : IUserService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UserService> _logger = logger;
    private readonly IEmailService _emailService = emailService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<(bool Success, List<Error>? Errors, User? User, string? Token)> CreateUserAsync(RegisterRequest request, Language language)
    {
        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = false,
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = _mapper.Map<List<Error>>(result.Errors);
            _logger.LogError("Failed to create user. Errors: {Errors}", errors);
            return (false, errors, null, null);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = System.Net.WebUtility.UrlEncode(token);

        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:3000";
        var confirmUrl = $"{frontendUrl}/confirm-email?userId={user.Id}&token={encodedToken}";

        var subject = "Confirm Your Email";
        var textBody = $"Click <a href='{confirmUrl}'>here</a> to confirm your email.";

        string templateName = "en-registration.html";
        if (language == Language.ES)
        {
            templateName = "es-registration.html";
        }
        var basePath = Directory.GetCurrentDirectory();
        string templatePath = Path.Combine(
            basePath, "EmailTemplates", templateName);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Email template not found: {templatePath}");
        }

        string emailTemplate = await File.ReadAllTextAsync(templatePath);
        string emailBody = emailTemplate.Replace("{confirmUrl}", confirmUrl);

        _logger.LogInformation("Sending email: ${Url}, ${body}", frontendUrl, textBody);

        EmailOptions options = new()
        {
            Subject = subject,
            TextBody = textBody,
            HtmlBody = emailBody,
            From = "Bohio - Verify Account <noreply@bohio.net>",
            To = user.Email
        };

        await _emailService.SendEmailAsync(options);

        _logger.LogInformation("Activation email sent to {Email}", user.Email);

        var userEntity = _mapper.Map<User>(user);
        return (true, null, userEntity, token);
    }
    public async Task<List<Error>?> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null) return [new Error("user_not_found", "User could not be found")];

        var decodedToken = System.Net.WebUtility.UrlDecode(request.Token);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            List<Error> errors = [.. result.Errors.Select(e => new Error(e.Code, e.Description))];
            return errors;
        }
        return null;
    }


    public async Task<(List<Error>? errors, AuthResponse response)> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return ([new("invalid_password", "The password was incorrect")], new AuthResponse { Message = "Could not login" });
        }

        var mappedUser = _mapper.Map<User>(user);
        var accessToken = GenerateAccessToken(mappedUser);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await _userManager.UpdateAsync(user);

        SetAuthCookies(accessToken, refreshToken);

        return (null, new AuthResponse { Message = "Successful login" });
    }

    public async Task<(List<Error>? errors, AuthResponse response)> RefreshTokenAsync(string refreshToken)
    {
        AuthResponse resp = new();
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            resp.Message = "Invalid or expired refresh token";
            return ([new("invalid_password", "the password was invalid")], new());
        }

        var mappedUser = _mapper.Map<User>(user);
        var accessToken = GenerateAccessToken(mappedUser);
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await _userManager.UpdateAsync(user);
        SetAuthCookies(accessToken, newRefreshToken);
        resp.Message = "successful login";

        return (null, resp);
    }

    public (List<Error>? errors, AuthResponse resp) Logout()
    {
        var resp = new AuthResponse();

        if (_httpContextAccessor.HttpContext == null)
        {
            var errors = new List<Error> { new("contextError", "Internal server error during logout") };
            return (errors, new AuthResponse { Message = "Could not logout" });
        }

        var response = _httpContextAccessor.HttpContext.Response;

        response.Cookies.Delete("AccessToken", new CookieOptions { HttpOnly = true, Secure = true });
        response.Cookies.Delete("RefreshToken", new CookieOptions { HttpOnly = true, Secure = true });

        resp.Message = "Successfully logged out";
        return (null, resp);
    }


    private string GenerateAccessToken(User user)
    {
        List<Claim> claims = [
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Ulid.NewUlid().ToString())
        ];
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
        if (string.IsNullOrEmpty(secret))
        {
            _logger.LogError("JWT_SECRET environment variable is missing.");
            throw new ArgumentException("missing jwt secret");
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "bohio-server",
            audience: "bohio-client",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private void SetAuthCookies(string accessToken, string refreshToken)
    {
        var context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is null");
        var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

        var accessTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(30)
        };

        context.Response.Cookies.Append("AccessToken", accessToken, accessTokenOptions);
        context.Response.Cookies.Append("RefreshToken", refreshToken, refreshTokenOptions); // Fixed: Use refreshToken
    }

    public async Task<(GetUserResponse? User, List<Error>? Errors)> GetUserByTokenAsync(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            return (new(), null);
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.ReadToken(accessToken) is not JwtSecurityToken tokenS)
            {
                return (null, new List<Error> { new("invalid_token", "The token is not valid") });
            }
            var userIdClaim = tokenS.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return (null, new List<Error> { new("invalid_token_claim", "The token does not contain a User Id claim") });
            }

            var userId = userIdClaim.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (null, new List<Error> { new("user_not_found", "User not found") });
            }

            var mappedUser = _mapper.Map<GetUserResponse>(user);

            return (mappedUser, null);

        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting user by token: {Error}", ex.Message);
            return (null, new List<Error> { new("internal_error", "An error occurred while processing token") });
        }
    }
}
