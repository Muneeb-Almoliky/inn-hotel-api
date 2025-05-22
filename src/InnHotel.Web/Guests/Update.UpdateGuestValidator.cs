using FastEndpoints;
using FluentValidation;

namespace InnHotel.Web.Guests;

public class UpdateGuestValidator : Validator<UpdateGuestRequest>
{
    public UpdateGuestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");

        RuleFor(x => x.IdProofType)
            .NotEmpty().WithMessage("ID proof type is required")
            .MaximumLength(50).WithMessage("ID proof type must not exceed 50 characters");

        RuleFor(x => x.IdProofNumber)
            .NotEmpty().WithMessage("ID proof number is required")
            .MaximumLength(50).WithMessage("ID proof number must not exceed 50 characters");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format");

        RuleFor(x => x.Phone)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Phone))
            .Matches(@"^\+?[0-9\-\s]*$").When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Invalid phone number format");

        RuleFor(x => x.Address)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Address))
            .WithMessage("Address must not exceed 200 characters");
    }
}