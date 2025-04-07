using Bohio.Core.DTOs;
using Bohio.Core.Interfaces;
using Bohio.Core.Results;
using Bohio.Core.Types;
using FluentValidation;
using Microsoft.Extensions.Logging;


namespace Bohio.Application.Services;
public class AuthService(
  IUserService userService,
  ILogger<AuthService> logger,
  IValidator<RegisterRequest> registerRequestValidator,
  IValidator<LoginRequest> loginRequestValidator,
  IValidator<ConfirmEmailRequest> confirmEmailValidator)
{
  private readonly IUserService _userService = userService;
  private readonly ILogger<AuthService> _logger = logger;
  private readonly IValidator<RegisterRequest> _registerRequestValidator = registerRequestValidator;
  private readonly IValidator<LoginRequest> _loginRequestValidator = loginRequestValidator;
  private readonly IValidator<ConfirmEmailRequest> _confirmEmailValidator = confirmEmailValidator;

  public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
  {
    var validationResult = await _confirmEmailValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      var validationErrors = validationResult.Errors.Select(x => new Error(x.PropertyName, x.ErrorMessage)).ToList();
      return Result.Failure(validationErrors);
    }

    var errors = await _userService.ConfirmEmailAsync(request);
    if (errors != null)
    {
      return Result.Failure(errors);
    }

    return Result.Success("Successfully registered");
  }

  public async Task<Result> RegisterUserAsync(RegisterRequest request, Language lang)
  {
    var validationResults = await _registerRequestValidator.ValidateAsync(request);
    if (!validationResults.IsValid)
    {
      var validationErrors = validationResults.Errors
          .Select(e => new Error(e.PropertyName, e.ErrorMessage))
          .ToList();
      return Result.Failure(validationErrors);
    }
    var (success, errors, user, token) = await _userService.CreateUserAsync(request, lang);
    if (!success)
    {
      return Result.Failure(errors!);
    }
    return Result.Success();
  }

  public async Task<Result> LoginAsync(LoginRequest request)
  {
    _logger.LogInformation("request: {email}", request.Email);
    var validationResults = await _loginRequestValidator.ValidateAsync(request);
    if (!validationResults.IsValid)
    {
      var valErrors = validationResults.Errors.Select(e => new Error(e.ErrorCode, e.ErrorMessage)).ToList();
      return Result.Failure(valErrors);
    }
    var (errors, response) = await _userService.LoginAsync(request);
    if (errors != null && errors.Count > 0)
    {
      return Result.Failure(errors!);
    }
    return Result.Success(response.Message);
  }
  public async Task<Result> RefreshTokenAsync(string token)
  {
    var (errors, response) = await _userService.RefreshTokenAsync(token);
    if (errors != null && errors.Count > 0)
    {
      return Result.Failure(errors!);
    }
    return Result.Success(response.Message);
  }
  public Result Logout()
  {
    var (errors, response) = _userService.Logout();
    return errors != null && errors.Count > 0
    ? Result.Failure(errors!)
    : Result.Success(response.Message);
  }

  public async Task<Result<GetUserResponse>> GetUserAsync(string accessToken)
  {
    var (userResponse, errors) = await _userService.GetUserByTokenAsync(accessToken);

    if (errors != null && errors.Count > 0)
    {
      return Result<GetUserResponse>.Failure(errors);
    }

    if (userResponse == null)
    {
      return Result<GetUserResponse>.Failure([new("internal_error", "user data was null")]);
    }

    return Result<GetUserResponse>.Success(userResponse);
  }
}

