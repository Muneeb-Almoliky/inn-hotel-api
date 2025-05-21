using InnHotel.Core.EmployeeAggregate;
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

        e.HasOne(e => e.User)
            .WithOne() 
            .HasForeignKey<Employee>(e => e.UserId)
            .IsRequired(false) 
            .OnDelete(DeleteBehavior.Cascade);
  }
}

