using InnHotel.Core.ServiceAggregate;

namespace InnHotel.Core.ReservationAggregate;

public class ReservationService(int reservationId, int serviceId, int quantity, decimal totalPrice)
  : EntityBase
{
    public int ReservationId { get; private set; } = reservationId;
    public Reservation Reservation { get; private set; } = null!;
    public int ServiceId { get; private set; } = Guard.Against.NegativeOrZero(serviceId, nameof(serviceId));
    public Service Service { get; private set; } = null!;
    public int Quantity { get; private set; } = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
    public decimal TotalPrice { get; private set; } = Guard.Against.NegativeOrZero(totalPrice, nameof(totalPrice));
}
