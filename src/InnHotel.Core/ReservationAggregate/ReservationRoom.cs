using InnHotel.Core.RoomAggregate;

namespace InnHotel.Core.ReservationAggregate;

public class ReservationRoom : EntityBase
{
    public ReservationRoom(int reservationId, int roomId, decimal pricePerNight)
    {
        ReservationId = reservationId;
        RoomId = Guard.Against.NegativeOrZero(roomId, nameof(roomId));
        PricePerNight = Guard.Against.NegativeOrZero(pricePerNight, nameof(pricePerNight));
    }

    protected ReservationRoom() : this(0, 1, 0.0m) { }

    public int ReservationId { get; }
    public Reservation Reservation { get; set; } = null!;
    public int RoomId { get; }
    public Room Room { get; set; } = null!;
    public decimal PricePerNight { get; }
}
