using FastEndpoints;
using FluentValidation;
using global::InnHotel.Web.Branches;

namespace InnHotel.Web.Branches;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class DeleteBranchValidator : Validator<DeleteBranchRequest>
{
  public DeleteBranchValidator()
  {
    RuleFor(x => x.BranchId)
      .GreaterThan(0);
  }
}
