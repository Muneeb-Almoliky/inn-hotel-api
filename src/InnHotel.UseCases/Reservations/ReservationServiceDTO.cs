namespace InnHotel.UseCases.Reservations;

public record ReservationServiceDTO(
    int ServiceId,
    int Quantity,
    decimal UnitPrice
); 