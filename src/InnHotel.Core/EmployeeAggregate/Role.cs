using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnHotel.Core.EmployeeAggregate;

public class Role(string name, string? description = null) : EntityBase
{
    // Constructor with 'this' initializer to satisfy CS8862
    protected Role() : this(string.Empty) { }

    public string Name { get; } = Guard.Against.NullOrEmpty(name, nameof(name));
    public string? Description { get; } = description;
}

