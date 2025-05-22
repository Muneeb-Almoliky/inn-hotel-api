using FastEndpoints;
using FluentValidation;
using global::InnHotel.Web.Guests;

namespace InnHotel.Web.Guests;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class DeleteGuestValidator : Validator<DeleteGuestRequest>
{
  public DeleteGuestValidator()
  {
    RuleFor(x => x.GuestId)
      .GreaterThan(0);
  }
}
