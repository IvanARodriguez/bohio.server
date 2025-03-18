using System.Text.RegularExpressions;
using Bohio.Core.DTOs;
using FluentValidation;

namespace Bohio.Application.Validators;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
  protected void ApplyEmailRules<TRequest>(IRuleBuilder<TRequest, string> ruleBuilder)
  {
    ruleBuilder
        .NotEmpty().WithMessage("Email is required.")
        .EmailAddress().WithMessage("Invalid email format.")
        .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.")
        .Must(email => email.Trim() == email).WithMessage("Email cannot have leading or trailing spaces.");
  }

  protected void ApplyPasswordRules<TRequest>(IRuleBuilder<TRequest, string> ruleBuilder)
  {
    ruleBuilder
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
        .MaximumLength(20).WithMessage("Password cannot exceed 20 characters.")
        .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
        .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
        .Matches(@"\d").WithMessage("Password must contain at least one number.")
        .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.")
        .Matches(@"^\S+$").WithMessage("Password cannot contain spaces.")
        .Must(password => !CommonPasswords.Contains(password)).WithMessage("Password is too weak.");
  }

  private static readonly HashSet<string> CommonPasswords = new()
    {
        "password", "123456", "qwerty", "abc123", "password1", "123456789", "letmein", "admin", "welcome"
    };
}

public class LoginValidator : BaseValidator<LoginRequest>
{
  public LoginValidator()
  {
    ApplyEmailRules(RuleFor(req => req.Email));
    ApplyPasswordRules(RuleFor(req => req.Password));
  }
}
public class RegisterValidator : BaseValidator<RegisterRequest>
{
  public RegisterValidator()
  {
    RuleFor(req => req.FirstName)
        .NotEmpty().WithMessage("First name is required.")
        .Matches(@"^[A-Za-zÀ-ÖØ-öø-ÿ\s'-]+$").WithMessage("First name contains invalid characters.")
        .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.")
        .Must(name => name.Trim() == name).WithMessage("First name cannot start or end with spaces.");

    RuleFor(req => req.LastName)
        .NotEmpty().WithMessage("Last name is required.")
        .Matches(@"^[A-Za-zÀ-ÖØ-öø-ÿ\s'-]+$").WithMessage("Last name contains invalid characters.")
        .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.")
        .Must(name => name.Trim() == name).WithMessage("Last name cannot start or end with spaces.");

    ApplyEmailRules(RuleFor(req => req.Email));
    ApplyPasswordRules(RuleFor(req => req.Password));
  }
}

public partial class ConfirmEmailValidator : AbstractValidator<ConfirmEmailRequest>
{
  private static readonly Regex UlidRegex = MyRegex();

  public ConfirmEmailValidator()
  {
    RuleFor(req => req.UserId.ToString())
        .NotEmpty().WithMessage("User ID is required.")
        .Matches(UlidRegex).WithMessage("Invalid ULID format.");

    RuleFor(req => req.Token)
        .NotEmpty().WithMessage("Identity token is required.")
        .MinimumLength(50).WithMessage("Invalid identity token format. Token is too short.")
        .Matches(@"^[a-zA-Z0-9%._\-]+$").WithMessage("Invalid identity token format. Unexpected characters found.");
  }

  [GeneratedRegex(@"^[0-7][0-9A-HJKMNP-TV-Z]{25}$", RegexOptions.Compiled)]
  private static partial Regex MyRegex();
}


