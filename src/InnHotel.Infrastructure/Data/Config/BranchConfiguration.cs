using InnHotel.Core.BranchAggregate;

namespace InnHotel.Infrastructure.Data.Config;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
  public void Configure(EntityTypeBuilder<Branch> b)
  {
    b.ToTable("branches");
    b.HasKey(x => x.Id).HasName("branch_id");

    b.Property(x => x.Name)
      .HasColumnName("name")
      .IsRequired()
      .HasMaxLength(100);

    b.Property(x => x.Location)
      .HasColumnName("location")
      .IsRequired()
      .HasMaxLength(200);

    // Relationships
    b.HasMany(x => x.RoomTypes)
     .WithOne().HasForeignKey(rt => rt.BranchId)
     .OnDelete(DeleteBehavior.Cascade);

    b.HasMany(x => x.Rooms)
     .WithOne().HasForeignKey(r => r.BranchId)
     .OnDelete(DeleteBehavior.Cascade);

    b.HasMany(x => x.Services)
     .WithOne().HasForeignKey(s => s.BranchId)
     .OnDelete(DeleteBehavior.Cascade);
  }
}
