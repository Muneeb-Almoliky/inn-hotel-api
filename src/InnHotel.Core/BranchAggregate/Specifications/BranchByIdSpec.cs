namespace InnHotel.Core.BranchAggregate.Specifications;

public class BranchByIdSpec : Specification<Branch>
{
    public BranchByIdSpec(int branchId) =>
        Query
            .Where(branch => branch.Id == branchId);
}