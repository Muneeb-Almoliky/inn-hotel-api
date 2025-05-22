namespace InnHotel.Core.GuestAggregate.Specifications;

public class GuestByIdProofNumberSpec : Specification<Guest>
{
    public GuestByIdProofNumberSpec(string idProofNumber) =>
        Query.Where(guest => guest.IdProofNumber == idProofNumber);
}