using FluentValidation;

namespace InnHotel.Web.Reservations;

public class CreateReservationValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationValidator()
    {
        RuleFor(x => x.GuestId)
            .GreaterThan(0)
            .WithMessage("GuestId must be greater than zero.");

        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("RoomId must be greater than zero.");

        RuleFor(x => x.NumberOfGuests)
            .GreaterThan(0)
            .WithMessage("Number of guests must be greater than zero.");

        RuleFor(x => x.CheckInDate)
            .GreaterThan(DateTime.UtcNow.Date)
            .WithMessage("Check-in date must be in the future.");

        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Check-out date must be after check-in date.");

    }
}
