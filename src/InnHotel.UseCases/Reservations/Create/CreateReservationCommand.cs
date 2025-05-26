using InnHotel.Core.ReservationAggregate;
using InnHotel.UseCases.Reservations;

public record CreateReservationCommand(
    int GuestId,
    string GuestFullName,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    ReservationStatus Status,
    IReadOnlyList<ReservationRoomDTO> Rooms,
    IReadOnlyList<ReservationServiceDTO> Services
) : ICommand<Result<ReservationDTO>>;
