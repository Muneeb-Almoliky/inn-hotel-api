using FastEndpoints;
using FluentValidation;

namespace InnHotel.Web.Rooms;

public class GetRoomByIdValidator : Validator<GetRoomByIdRequest>
{
    public GetRoomByIdValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("Room ID must be greater than 0");
    }
}