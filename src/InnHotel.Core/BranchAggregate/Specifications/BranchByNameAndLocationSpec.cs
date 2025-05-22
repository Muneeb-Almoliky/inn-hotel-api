namespace InnHotel.Core.BranchAggregate.Specifications;

public class BranchByNameAndLocationSpec : Specification<Branch>
{
    public BranchByNameAndLocationSpec(string name, string location) =>
        Query.Where(branch => branch.Name == name && branch.Location == location);
}