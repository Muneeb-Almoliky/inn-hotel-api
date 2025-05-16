using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnHotel.Core.BranchAggregate;

namespace InnHotel.Core.ServiceAggregate;

public class Service : EntityBase
{
    public Service(int branchId, string name, decimal price, string? description = null)
    {
        BranchId = Guard.Against.NegativeOrZero(branchId, nameof(branchId));
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Price = Guard.Against.Negative(price, nameof(price));
        Description = description;
    }

    protected Service() : this(0, string.Empty, 0) { }

    public int BranchId { get; }
    public Branch Branch { get; set; } = null!;
    public string Name { get; }
    public decimal Price { get; }
    public string? Description { get; }
}
