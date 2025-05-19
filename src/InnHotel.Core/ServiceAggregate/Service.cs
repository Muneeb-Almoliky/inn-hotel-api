using InnHotel.Core.BranchAggregate;

namespace InnHotel.Core.ServiceAggregate;

public class Service(int branchId, string name, decimal price, string? description = null) : EntityBase
{
    public int BranchId { get; private set; } = Guard.Against.NegativeOrZero(branchId, nameof(branchId));
    public Branch Branch { get; private set; } = null!;
    public string Name { get; private set; } = Guard.Against.NullOrEmpty(name, nameof(name));
    public decimal Price { get; private set; } = Guard.Against.Negative(price, nameof(price));
    public string? Description { get; private set; } = description;
}
