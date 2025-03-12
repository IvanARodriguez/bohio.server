using Homespirations.Core.DTOs;
using Homespirations.Core.Results;
using Homespirations.Core.Types;

namespace Homespirations.Core.Interfaces;

public interface IUserService
{
    Task<(bool Success, List<Error>? Errors, User? User, string? Token)> CreateUserAsync(RegisterRequest request, Language language);
    Task<bool> ConfirmEmailAsync(string userId, string token);
}
