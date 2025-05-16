using InnHotel.Core.EmployeeAggregate;
using InnHotel.Core.BranchAggregate;
namespace InnHotel.Infrastructure.Data.Config;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> e)
    {
        e.ToTable(t =>
        {
            t.HasCheckConstraint(
              "CK_employees_hiredate",
              "hire_date <= CURRENT_DATE");
        });

        e.ToTable("employees");
        e.HasKey(x => x.Id).HasName("employee_id");

        e.Property(x => x.BranchId)
          .HasColumnName("branch_id")
          .IsRequired();

        e.Property(x => x.FirstName)
          .HasColumnName("first_name")
          .IsRequired()
          .HasMaxLength(50);

        e.Property(x => x.LastName)
          .HasColumnName("last_name")
          .IsRequired()
          .HasMaxLength(50);

        e.Property(x => x.Email)
          .HasColumnName("email")
          .IsRequired()
          .HasMaxLength(100);
        e.HasIndex(x => x.Email)
         .IsUnique()
         .HasDatabaseName("UX_employees_email");

        e.Property(x => x.Phone)
          .HasColumnName("phone")
          .HasMaxLength(20);

        e.Property(x => x.HireDate)
          .HasColumnName("hire_date")
          .IsRequired();

        e.Property(x => x.Position)
          .HasColumnName("position")
          .IsRequired()
          .HasMaxLength(50);

        e.HasOne(x => x.Branch)
          .WithMany()
          .HasForeignKey(x => x.BranchId)
          .OnDelete(DeleteBehavior.Cascade);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
  public void Configure(EntityTypeBuilder<Role> r)
  {
    r.ToTable("roles");
    r.HasKey(x => x.Id).HasName("role_id");

    r.Property(x => x.Name)
      .HasColumnName("role_name")
      .IsRequired()
      .HasMaxLength(50);
    r.HasIndex(x => x.Name)
     .IsUnique()
     .HasDatabaseName("UX_roles_rolename");

    r.Property(x => x.Description)
      .HasColumnName("description");
  }
}

public class EmployeeRoleConfiguration : IEntityTypeConfiguration<EmployeeRole>
{
  public void Configure(EntityTypeBuilder<EmployeeRole> er)
  {
    er.ToTable("employee_roles");
    er.HasKey(x => new { x.EmployeeId, x.RoleId });
  }
}
