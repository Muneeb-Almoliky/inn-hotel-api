using FluentValidation;

namespace InnHotel.Web.Auth.Register;

public class RegisterValidator : Validator<RegisterRequest>
{
  public RegisterValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress();

    RuleFor(x => x.Password)
        .NotEmpty()
        .MinimumLength(8);

    RuleFor(x => x.FirstName)
        .NotEmpty()
        .MaximumLength(50);

    RuleFor(x => x.LastName)
        .NotEmpty()
        .MaximumLength(50);
  }
}
