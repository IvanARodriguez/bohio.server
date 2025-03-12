using Bohio.Core.DTOs;
using Bohio.Core.Interfaces;
using Bohio.Core.Results;
using Bohio.Core.Types;
using Microsoft.Extensions.Logging;


namespace Bohio.Application.Services;
public class AuthService(IUserService userService, ILogger<AuthService> logger)
{
    private readonly IUserService _userService = userService;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<Result> RegisterUserAsync(RegisterRequest request, Language lang)
    {
        var (success, errors, user, token) = await _userService.CreateUserAsync(request, lang);
        if (!success)
        {
            return Result.Failure(errors!);
        }

        _logger.LogInformation("User registered: {UserId}. Activation email sent.", user!.Id);
        return Result.Success();
    }
}

