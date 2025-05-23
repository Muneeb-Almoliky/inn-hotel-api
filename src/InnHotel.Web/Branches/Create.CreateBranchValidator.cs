using FastEndpoints;
using FluentValidation;

namespace InnHotel.Web.Branches;

public class CreateBranchValidator : Validator<CreateBranchRequest>
{
    public CreateBranchValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Branch name is required")
            .MinimumLength(2).WithMessage("Branch name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Branch name must not exceed 100 characters");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MinimumLength(2).WithMessage("Location must be at least 2 characters")
            .MaximumLength(200).WithMessage("Location must not exceed 200 characters");
    }
}