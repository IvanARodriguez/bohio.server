using Homespirations.Core.DTOs;
using Homespirations.Core.Results;

namespace Homespirations.Core.Interfaces;

public interface IUserService
{
    Task<(bool Success, List<Error>? Errors, User? User, string? Token)> CreateUserAsync(RegisterRequest request);
    Task<bool> ConfirmEmailAsync(string userId, string token);
}
