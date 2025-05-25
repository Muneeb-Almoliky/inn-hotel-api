using Ardalis.Specification;

namespace InnHotel.Core.ReservationAggregate.Specifications;

public class ReservationByGuestSpec : Specification<Reservation>
{
    public ReservationByGuestSpec(int guestId)
    {
        Query.Where(r => r.GuestId == guestId);
    }
}
