namespace InnHotel.Core.EmployeeAggregate;

public class EmployeeRole(int employeeId, int roleId) : EntityBase
{
    public int EmployeeId { get; private set; } = Guard.Against.NegativeOrZero(employeeId, nameof(employeeId));
    public int RoleId { get; private set; } = Guard.Against.NegativeOrZero(roleId, nameof(roleId));
}
