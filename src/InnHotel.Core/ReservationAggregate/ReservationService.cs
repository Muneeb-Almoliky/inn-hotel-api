using InnHotel.Core.ServiceAggregate;

namespace InnHotel.Core.ReservationAggregate;

public class ReservationService(int reservationId, int serviceId, int quantity, decimal totalPrice)
  : EntityBase
{
    protected ReservationService()
        : this(default, default, default, default) { }

    public int ReservationId { get; } = reservationId;
    public Reservation Reservation { get; set; } = null!;
    public int ServiceId { get; } = Guard.Against.NegativeOrZero(serviceId, nameof(serviceId));
    public Service Service { get; set; } = null!;
    public int Quantity { get; } = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
    public decimal TotalPrice { get; } = Guard.Against.NegativeOrZero(totalPrice, nameof(totalPrice));
}
