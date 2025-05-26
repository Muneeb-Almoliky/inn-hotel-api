using InnHotel.Core.GuestAggregate;

namespace InnHotel.Core.ReservationAggregate;

public class Reservation(
    int guestId,
    DateOnly checkInDate,
    DateOnly checkOutDate,
    ReservationStatus status
) : EntityBase, IAggregateRoot
{
  public int GuestId { get; private set; } = Guard.Against.NegativeOrZero(guestId, nameof(guestId));
  public Guest Guest { get; private set; } = null!;
  public DateOnly CheckInDate { get; private set; } = checkInDate;
  public DateOnly CheckOutDate { get; private set; } = Guard.Against.OutOfRange(
      checkOutDate, nameof(checkOutDate), checkInDate.AddDays(1), DateOnly.MaxValue);
  public DateTime ReservationDate { get; private set; } = DateTime.UtcNow;
  public ReservationStatus Status { get; private set; } = status;
  public decimal TotalCost { get; private set; } = 0m;  // default to zero

  private readonly List<ReservationRoom> _rooms = new();
  private readonly List<ReservationService> _services = new();
  public IReadOnlyCollection<ReservationRoom> Rooms => _rooms.AsReadOnly();
  public IReadOnlyCollection<ReservationService> Services => _services.AsReadOnly();

  public void AddRoom(int roomId, decimal pricePerNight)
  {
    _rooms.Add(new ReservationRoom(roomId, pricePerNight));
    TotalCost += pricePerNight;
  }

  public void AddService(int serviceId, int quantity, decimal unitPrice)
  {
    _services.Add(new ReservationService(Id, serviceId, quantity, unitPrice));
    TotalCost += quantity * unitPrice;
  }
}
