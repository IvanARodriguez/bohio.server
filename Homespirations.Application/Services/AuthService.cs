using Homespirations.Core.DTOs;
using Homespirations.Core.Interfaces;
using Homespirations.Core.Results;
using Microsoft.Extensions.Logging;


namespace Homespirations.Application.Services;
public class AuthService(IUserService userService, ILogger<AuthService> logger)
{
    private readonly IUserService _userService = userService;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<Result> RegisterUserAsync(RegisterRequest request)
    {
        var (success, errors, user, token) = await _userService.CreateUserAsync(request);
        if (!success)
        {
            return Result.Failure(errors!);
        }

        _logger.LogInformation("User registered: {UserId}. Activation email sent.", user!.Id);
        return Result.Success();
    }
}

