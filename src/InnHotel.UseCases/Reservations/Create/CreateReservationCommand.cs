using InnHotel.UseCases.Reservations;

public record CreateReservationCommand(
    int GuestId,
    int RoomId,
    DateOnly CheckInDate,
    DateOnly CheckOutDate
) : ICommand<Result<ReservationDTO>>;
