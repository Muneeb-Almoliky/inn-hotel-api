namespace InnHotel.UseCases.Reservations;

public record ReservationDTO(
    int Id,
    int GuestId,
    string GuestFullName,
    string Status,
    int RoomId,
    string RoomNumber,
    decimal PricePerNight,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    decimal TotalCost,
    int NumberOfGuests,
    string? Notes
);
