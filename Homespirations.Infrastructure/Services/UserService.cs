
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Homespirations.Core.DTOs;
using Homespirations.Core.Results;
using Homespirations.Core.Interfaces;
using Homespirations.Infrastructure.Identity;

public class UserService(UserManager<AppUser> userManager, IMapper mapper, ILogger<UserService> logger) : IUserService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<(bool Success, List<Error>? Errors, User? User)> CreateUserAsync(RegisterRequest request)
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
            return (false, errors, null);
        }

        var userEntity = _mapper.Map<User>(user);

        return (true, null, userEntity);
    }
}
