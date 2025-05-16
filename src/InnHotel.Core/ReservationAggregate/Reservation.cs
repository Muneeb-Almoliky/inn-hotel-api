using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnHotel.Core.BranchAggregate;
using InnHotel.Core.GuestAggregate;

namespace InnHotel.Core.ReservationAggregate;

public class Reservation(
    int guestId,
    DateOnly checkInDate,
    DateOnly checkOutDate,
    ReservationStatus status,
    decimal totalCost
) : EntityBase, IAggregateRoot
{
    protected Reservation() : this(default, default, default, default, default) { }

    public int GuestId { get; } = Guard.Against.NegativeOrZero(guestId, nameof(guestId));
    public Guest Guest { get; set; } = null!;
    public DateOnly CheckInDate { get; } = checkInDate;
    public DateOnly CheckOutDate { get; } = Guard.Against.OutOfRange(
        checkOutDate, nameof(checkOutDate), checkInDate.AddDays(1), DateOnly.MaxValue);
    public DateTime ReservationDate { get; } = DateTime.UtcNow;
    public ReservationStatus Status { get; private set; } = status;
    public decimal TotalCost { get; } = Guard.Against.Negative(totalCost, nameof(totalCost));

    private readonly List<ReservationRoom> _rooms = new();
    private readonly List<ReservationService> _services = new();

    public IReadOnlyCollection<ReservationRoom> Rooms => _rooms.AsReadOnly();
    public IReadOnlyCollection<ReservationService> Services => _services.AsReadOnly();

    public void AddRoom(int roomId, decimal pricePerNight)
      => _rooms.Add(new ReservationRoom(Id, roomId, pricePerNight));

    public void AddService(int serviceId, int quantity, decimal unitPrice)
      => _services.Add(new ReservationService(Id, serviceId, quantity, unitPrice * quantity));
}
