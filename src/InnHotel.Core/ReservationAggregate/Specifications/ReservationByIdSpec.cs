using Ardalis.Specification;

namespace InnHotel.Core.ReservationAggregate.Specifications;

public class ReservationByIdSpec : Specification<Reservation>, ISingleResultSpecification 
{
    public ReservationByIdSpec(int reservationId)
    {
        Query.Where(r => r.Id == reservationId);
    }
}
