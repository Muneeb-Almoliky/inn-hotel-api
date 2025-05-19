using InnHotel.Core.BranchAggregate;

namespace InnHotel.Core.EmployeeAggregate;

public class Employee(
    int branchId,
    string firstName,
    string lastName,
    string email,
    DateOnly hireDate,
    string position,
    string? phone = null
) : EntityBase, IAggregateRoot
{
    public int BranchId { get; } = Guard.Against.NegativeOrZero(branchId, nameof(branchId));
    public Branch Branch { get; set; } = null!; 
    public string FirstName { get; } = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
    public string LastName { get; } = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
    public string Email { get; } = email;
    public string? Phone { get; private set; } = phone;
    public DateOnly HireDate { get; } = hireDate;
    public string Position { get; } = Guard.Against.NullOrEmpty(position, nameof(position));

    private readonly List<EmployeeRole> _roles = new();
    public IReadOnlyCollection<EmployeeRole> Roles => _roles.AsReadOnly();
    public void AssignRole(Role role)
      => _roles.Add(new EmployeeRole(Id, role.Id));
}
