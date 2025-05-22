using FastEndpoints;
using FluentValidation;

namespace InnHotel.Web.Guests;

public class GetGuestByIdValidator : Validator<GetGuestByIdRequest>
{
  public GetGuestByIdValidator()
  {
    RuleFor(x => x.GuestId)
      .GreaterThan(0);
  }
}
