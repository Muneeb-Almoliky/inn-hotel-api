using InnHotel.Core.RoomAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnHotel.Infrastructure.Data.Config;

  public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
  {
      public void Configure(EntityTypeBuilder<RoomType> rt)
      {
          rt.ToTable(t =>
          {
              t.HasCheckConstraint("CK_room_types_base_price", "base_price > 0");
              t.HasCheckConstraint("CK_room_types_capacity", "capacity > 0");
          });

          rt.HasKey(x => x.Id).HasName("room_type_id");

          rt.Property(x => x.BranchId)
            .HasColumnName("branch_id")
            .IsRequired();

          rt.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(50);

          rt.Property(x => x.Description)
            .HasColumnName("description");

          rt.Property(x => x.BasePrice)
            .HasColumnName("base_price")
            .HasColumnType("numeric(10,2)")
            .IsRequired();

          rt.Property(x => x.Capacity)
            .HasColumnName("capacity")
            .IsRequired();

          rt.HasOne(x => x.Branch)
            .WithMany(b => b.RoomTypes)
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Cascade);
      }
  }
