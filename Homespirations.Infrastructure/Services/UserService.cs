using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Homespirations.Core.DTOs;
using Homespirations.Core.Results;
using Homespirations.Core.Interfaces;
using Homespirations.Infrastructure.Identity;

public class UserService(UserManager<AppUser> userManager, IMapper mapper, ILogger<UserService> logger, IEmailService emailService) : IUserService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UserService> _logger = logger;
    private readonly IEmailService _emailService = emailService;

    public async Task<(bool Success, List<Error>? Errors, User? User, string? Token)> CreateUserAsync(RegisterRequest request)
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

        // Generate Email Confirmation Token
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = System.Net.WebUtility.UrlEncode(token); // Make it URL-safe

        // Use Environment Variable for Frontend URL
        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:3000";
        var confirmUrl = $"{frontendUrl}/confirm-email?userId={user.Id}&token={encodedToken}";

        // Send Activation Email
        var subject = "Confirm Your Email";
        var body = $"Click <a href='{confirmUrl}'>here</a> to confirm your email.";
        _logger.LogInformation("Sending email: ${Url}, ${body}", frontendUrl, body);
        await _emailService.SendEmailAsync(user.Email, subject, body, body);

        _logger.LogInformation("Activation email sent to {Email}", user.Email);

        var userEntity = _mapper.Map<User>(user);
        return (true, null, userEntity, token);
    }
    public async Task<bool> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }
}
