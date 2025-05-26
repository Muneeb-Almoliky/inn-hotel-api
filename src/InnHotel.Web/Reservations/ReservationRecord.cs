using InnHotel.UseCases.Reservations;

namespace InnHotel.Web.Reservations;

public record ReservationRecord(
    int Id,
    int GuestId,
    string GuestFullName,
    ReservationStatus Status,
    IReadOnlyList<ReservationRoomDTO> Rooms,
    IReadOnlyList<ReservationServiceDTO> Services,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    decimal TotalCost
);
