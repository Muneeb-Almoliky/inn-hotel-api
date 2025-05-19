using InnHotel.Core.RoomAggregate;

namespace InnHotel.Core.ReservationAggregate;

public class ReservationRoom(int reservationId, int roomId, decimal pricePerNight) : EntityBase
{
    public int ReservationId { get; private set;  } = Guard.Against.NegativeOrZero(reservationId, nameof(reservationId));
    public Reservation Reservation { get; private set; } = null!;
    public int RoomId { get; private set; } = Guard.Against.NegativeOrZero(roomId, nameof(roomId));
    public Room Room { get; private set; } = null!;
    public decimal PricePerNight { get; private set; } = Guard.Against.NegativeOrZero(pricePerNight, nameof(pricePerNight));
}
