using Ardalis.Specification;
using System;

namespace InnHotel.Core.ReservationAggregate.Specifications;

public class ReservationByDateRangeSpec : Specification<Reservation>
{
    public ReservationByDateRangeSpec(DateTime startDate, DateTime endDate)
    {
        Query.Where(r =>
            (r.CheckInDate < endDate) && (r.CheckOutDate > startDate)
        );
    }
}
