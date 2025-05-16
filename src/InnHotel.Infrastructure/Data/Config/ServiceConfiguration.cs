using InnHotel.Core.ServiceAggregate;

namespace InnHotel.Infrastructure.Data.Config;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> s)
    {
        s.ToTable("services", t =>
        {
            t.HasCheckConstraint("CK_services_price", "price >= 0");
        });

        s.HasKey(x => x.Id).HasName("service_id");

        s.Property(x => x.BranchId)
          .HasColumnName("branch_id")
          .IsRequired();

        s.Property(x => x.Name)
          .HasColumnName("name")
          .IsRequired()
          .HasMaxLength(100);

        s.Property(x => x.Description)
          .HasColumnName("description");

        s.Property(x => x.Price)
          .HasColumnName("price")
          .HasColumnType("numeric(10,2)")
          .IsRequired();

        s.HasOne(x => x.Branch)
          .WithMany(b => b.Services)
          .HasForeignKey(x => x.BranchId)
          .OnDelete(DeleteBehavior.Cascade);
    }
}
