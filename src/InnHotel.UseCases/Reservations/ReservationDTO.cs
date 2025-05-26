namespace InnHotel.UseCases.Reservations;

public record ReservationDTO(
    int Id,
    int GuestId,
    string GuestFullName,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    ReservationStatus Status,
    decimal TotalCost,
    List<ReservationRoomDTO> Rooms,
    List<ReservationServiceDTO> Services
);

//public record ReservationRoomDTO(int RoomId, decimal PricePerNight);
//public record ReservationServiceDTO(int ServiceId, int Quantity, decimal TotalPrice);
