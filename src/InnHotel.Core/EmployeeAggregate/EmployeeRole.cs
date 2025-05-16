namespace InnHotel.Core.EmployeeAggregate;

public class EmployeeRole : EntityBase
{
    public EmployeeRole(int employeeId, int roleId)
    {
        EmployeeId = Guard.Against.NegativeOrZero(employeeId, nameof(employeeId));
        RoleId = Guard.Against.NegativeOrZero(roleId, nameof(roleId));
    }

    protected EmployeeRole() : this(0, 0) { }

    public int EmployeeId { get; }
    public int RoleId { get; }
}
