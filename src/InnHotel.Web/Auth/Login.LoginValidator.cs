using FluentValidation;

namespace InnHotel.Web.Auth;

public class LoginValidator : AbstractValidator<LoginRequest>
{
  public LoginValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty()
        .WithMessage("Email is required")
        .EmailAddress()
        .WithMessage("Invalid email format");

    RuleFor(x => x.Password)
        .NotEmpty()
        .WithMessage("Password is required")
        .MinimumLength(8)
        .WithMessage("Password must be at least 8 characters");
  }
}
