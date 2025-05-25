using Ardalis.Specification;
using InnHotel.Core.ReservationAggregate;

namespace InnHotel.Core.ReservationAggregate.Specifications;

public class ReservationByRoomAndDateSpec : Specification<Reservation>
{
  public ReservationByRoomAndDateSpec(int roomId, DateOnly checkIn, DateOnly checkOut)
  {
    Query.Where(r =>
        r.Rooms.Any(rr => rr.RoomId == roomId) &&
        r.CheckInDate < checkOut &&
        r.CheckOutDate > checkIn
    );
  }
}
