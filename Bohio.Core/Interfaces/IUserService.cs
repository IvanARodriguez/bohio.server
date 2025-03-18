using Bohio.Core.DTOs;
using Bohio.Core.Results;
using Bohio.Core.Types;

namespace Bohio.Core.Interfaces;

public interface IUserService
{
    Task<(bool Success, List<Error>? Errors, User? User, string? Token)> CreateUserAsync(RegisterRequest request, Language language);
    Task<List<Error>?> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<(List<Error>? errors, AuthResponse response)> LoginAsync(LoginRequest request);
    Task<(List<Error>? errors, AuthResponse response)> RefreshTokenAsync(string refreshToken);
    (List<Error>? errors, AuthResponse resp) Logout();

    Task<(GetUserResponse? User, List<Error>? Errors)> GetUserByTokenAsync(string accessToken);
}
