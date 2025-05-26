using Ardalis.Specification;

namespace InnHotel.Core.ReservationAggregate.Specifications;

public class RoomAvailabilitySpec : Specification<Reservation>
{
    public RoomAvailabilitySpec(int roomId, DateOnly checkInDate, DateOnly checkOutDate)
    {
        Query
            .Include(r => r.Rooms)
            .Where(r => r.Rooms.Any(rr => rr.RoomId == roomId))
            .Where(r => r.Status != ReservationStatus.Cancelled)
            .Where(r => 
                // Check if there's any overlap between the requested dates and existing reservations
                (checkInDate <= r.CheckOutDate && checkOutDate >= r.CheckInDate)
            );
    }
} 