namespace InnHotel.Core.GuestAggregate.Specifications;

public class GuestByProofNumberSpec : Specification<Guest>
{
  public GuestByProofNumberSpec(string proofNumber) =>
      Query.Where(g => g.IdProofNumber == proofNumber);
}
