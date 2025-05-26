using FluentValidation;

namespace InnHotel.Web.Reservations;

public class CreateReservationValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationValidator()
    {
        RuleFor(x => x.GuestId)
            .GreaterThan(0)
            .WithMessage("Guest ID must be greater than 0");

        RuleFor(x => x.CheckInDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Check-in date must be today or in the future");

        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Check-out date must be after check-in date");

        RuleFor(x => x.Rooms)
            .NotEmpty()
            .WithMessage("At least one room must be specified");

        RuleForEach(x => x.Rooms)
            .ChildRules(room =>
            {
                room.RuleFor(x => x.RoomId)
                    .GreaterThan(0)
                    .WithMessage("Room ID must be greater than 0");

                room.RuleFor(x => x.PricePerNight)
                    .GreaterThan(0)
                    .WithMessage("Price per night must be greater than 0");
            });

        RuleForEach(x => x.Services)
            .ChildRules(service =>
            {
                service.RuleFor(x => x.ServiceId)
                    .GreaterThan(0)
                    .WithMessage("Service ID must be greater than 0");

                service.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0");

                service.RuleFor(x => x.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("Unit price must be greater than 0");
            });
    }
}
