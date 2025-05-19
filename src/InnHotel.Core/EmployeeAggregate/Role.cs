namespace InnHotel.Core.EmployeeAggregate;

public class Role(string name, string? description = null) : EntityBase
{
    public string Name { get; private set; } = Guard.Against.NullOrEmpty(name, nameof(name));
    public string? Description { get; private set; } = description;
}

