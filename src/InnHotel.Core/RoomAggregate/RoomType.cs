using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnHotel.Core.BranchAggregate;
using Ardalis.SharedKernel;

namespace InnHotel.Core.RoomAggregate;

public class RoomType(int branchId, string name, decimal basePrice, int capacity, string? description = null) : EntityBase, IAggregateRoot
{
    public int BranchId { get; private set; } = Guard.Against.NegativeOrZero(branchId, nameof(branchId));
    public Branch Branch { get; private set; } = null!;
    public string Name { get; private set; } = Guard.Against.NullOrEmpty(name, nameof(name));
    public decimal BasePrice { get; private set; } = Guard.Against.NegativeOrZero(basePrice, nameof(basePrice));
    public int Capacity { get; private set; } = Guard.Against.NegativeOrZero(capacity, nameof(capacity));
    public string? Description { get; } = description;
}
