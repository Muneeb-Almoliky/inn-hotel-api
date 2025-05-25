namespace InnHotel.UseCases.Reservations;

public record ReservationDTO(
    int Id,
    int GuestId,
    string Status,
    int RoomId,
    decimal PricePerNight,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    decimal TotalCost,
    int NumberOfGuests,
    string? Notes
);

