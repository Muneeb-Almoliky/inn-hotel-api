using FastEndpoints;
using FluentValidation;

namespace InnHotel.Web.Rooms;

/// <summary>
/// Validates incoming CreateRoomRequest
/// </summary>
public class CreateRoomValidator : Validator<CreateRoomRequest>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.BranchId)
            .GreaterThan(0).WithMessage("Branch ID must be greater than 0");

        RuleFor(x => x.RoomTypeId)
            .GreaterThan(0).WithMessage("Room Type ID must be greater than 0");

        RuleFor(x => x.RoomNumber)
            .NotEmpty().WithMessage("Room number is required")
            .MaximumLength(20).WithMessage("Room number must not exceed 20 characters");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid room status");

        RuleFor(x => x.Floor)
            .GreaterThanOrEqualTo(0).WithMessage("Floor must be 0 or greater")
            .LessThanOrEqualTo(100).WithMessage("Floor must not exceed 100");
    }
}