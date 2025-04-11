using System.ComponentModel.DataAnnotations;
using Bohio.Application.Services;
using Bohio.Core.DTOs;
using Bohio.Core.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using FluentValidation.Results;
using Moq;
using Bohio.Core.Types;

namespace Bohio.Api.IntegrationTests;

public partial class AuthenticationTests : IClassFixture<BohioWebAppFactory>
{
  private readonly Mock<IUserService> _userServiceMock = new();
  private readonly Mock<ILogger<AuthService>> _loggerMock = new();
  private readonly Mock<IValidator<RegisterRequest>> _registerValidatorMock = new();
  private readonly Mock<IValidator<LoginRequest>> _loginValidatorMock = new();
  private readonly Mock<IValidator<ConfirmEmailRequest>> _confirmEmailValidatorMock = new();

  private readonly AuthService _authService;

  public AuthenticationTests()
  {
    _authService = new AuthService(
        _userServiceMock.Object,
        _loggerMock.Object,
        _registerValidatorMock.Object,
        _loginValidatorMock.Object,
        _confirmEmailValidatorMock.Object
    );
  }

  [Fact]
  public async Task RegisterUserAsync_ReturnsSuccess_WhenValidRequest()
  {
    // Arrange
    var request = new RegisterRequest
    {
      Email = "ivan@hitab.dev",
      Password = "ValidPassw0rd1991!",
      FirstName = "Test",
      LastName = "User"
    };

    _registerValidatorMock
    .Setup(v => v.ValidateAsync(request, default))
    .ReturnsAsync(new FluentValidation.Results.ValidationResult());

    _userServiceMock
    .Setup(u => u.CreateUserAsync(request, Language.EN))
    .ReturnsAsync((true, null, new User(), "token"));

    // Act
    var result = await _authService.RegisterUserAsync(request, Language.EN);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Empty(result.Errors ?? []);
  }

  [Fact]
  public async Task LoginUserAsync_ReturnSuccess_WhenValidRequest()
  {
    // Arrange
    var request = new LoginRequest
    {
      Email = "ivan@hitab.dev",
      Password = "ValidP@ssw0rd1991!"
    };

    _loginValidatorMock
    .Setup(v => v.ValidateAsync(request, default))
    .ReturnsAsync(new FluentValidation.Results.ValidationResult());

    _userServiceMock
    .Setup(u => u.LoginAsync(request))
    .ReturnsAsync((null, new AuthResponse { Message = "" }));

    // Act
    var result = await _authService.LoginAsync(request);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Empty(result.Errors ?? []);
  }

  public static IEnumerable<object[]> InvalidPasswords =>
[
    [new LoginRequest { Email = "ivan@hitab.dev", Password = "" }], // empty
    [new LoginRequest { Email = "ivan@hitab.dev", Password = "short" }], // too short
    [new LoginRequest { Email = "ivan@hitab.dev", Password = "123456" }], // common
    [new LoginRequest { Email = "ivan@hitab.dev", Password = "Password" }], // missing number & special char
    [new LoginRequest { Email = "ivan@hitab.dev", Password = "Password1" }], // missing special char
];

  [Theory]
  [MemberData(nameof(InvalidPasswords))]
  public async Task LoginUserAsync_ReturnError_WhenInvalidPassword(LoginRequest creds)
  {
    // Arrange
    var validationFailure = new ValidationFailure(nameof(LoginRequest.Password), "Invalid password");
    var validationResult = new FluentValidation.Results.ValidationResult(new List<ValidationFailure> { validationFailure });

    _loginValidatorMock
        .Setup(v => v.ValidateAsync(creds, default))
        .ReturnsAsync(validationResult);

    // Act
    var result = await _authService.LoginAsync(creds);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Contains(result.Errors, e => e.Message.Contains("Invalid password", StringComparison.OrdinalIgnoreCase));
  }
}
