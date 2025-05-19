using InnHotel.Core.BranchAggregate;

namespace InnHotel.Core.EmployeeAggregate;

public class Employee(
    int branchId,
    string firstName,
    string lastName,
    DateOnly hireDate,
    string position
) : EntityBase, IAggregateRoot
{
    public int BranchId { get; } = Guard.Against.NegativeOrZero(branchId, nameof(branchId));
    public Branch Branch { get; set; } = null!; 
    public string FirstName { get; } = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
    public string LastName { get; } = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
    public DateOnly HireDate { get; } = hireDate;
    public string Position { get; } = Guard.Against.NullOrEmpty(position, nameof(position));

}
