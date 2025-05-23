using FastEndpoints;
using FluentValidation;
using global::InnHotel.Web.Rooms;

namespace InnHotel.Web.Rooms;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class DeleteRoomValidator : Validator<DeleteRoomRequest>
{
  public DeleteRoomValidator()
  {
    RuleFor(x => x.RoomId)
      .GreaterThan(0);
  }
}