using Bohio.Core.DTOs;
using Bohio.Core.Results;
using Bohio.Core.Types;

namespace Bohio.Core.Interfaces;

public interface IUserService
{
    Task<(bool Success, List<Error>? Errors, User? User, string? Token)> CreateUserAsync(RegisterRequest request, Language language);
    Task<bool> ConfirmEmailAsync(string userId, string token);
}
