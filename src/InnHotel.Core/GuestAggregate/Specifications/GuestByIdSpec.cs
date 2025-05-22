namespace InnHotel.Core.GuestAggregate.Specifications;

public class GuestByIdSpec : Specification<Guest>
{
  public GuestByIdSpec(int guestId) =>
    Query
        .Where(guest => guest.Id == guestId);
}
