using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnHotel.Core.BranchAggregate;

namespace InnHotel.Core.RoomAggregate;

public class RoomType : EntityBase
{
    public RoomType(int branchId, string name, decimal basePrice, int capacity, string? description = null)
    {
        BranchId = Guard.Against.NegativeOrZero(branchId, nameof(branchId));
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        BasePrice = Guard.Against.NegativeOrZero(basePrice, nameof(basePrice));
        Capacity = Guard.Against.NegativeOrZero(capacity, nameof(capacity));
        Description = description;
    }

    protected RoomType() : this(0, string.Empty, 0, 0, null) { }

    public int BranchId { get; }
    public Branch Branch { get; set; } = null!;
    public string Name { get; }
    public decimal BasePrice { get; }
    public int Capacity { get; }
    public string? Description { get; }
}
