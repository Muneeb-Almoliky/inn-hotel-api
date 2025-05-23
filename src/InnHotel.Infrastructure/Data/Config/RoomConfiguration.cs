using InnHotel.Core.RoomAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnHotel.Infrastructure.Data.Config;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> r)
    {
        r.ToTable("rooms", t =>
        {
            t.HasCheckConstraint("CK_rooms_status", "status IN ('Available','Occupied','UnderMaintenance')");
            t.HasCheckConstraint("CK_rooms_floor", "floor >= 0");
        });

        r.HasKey(x => x.Id).HasName("room_id");

        r.Property(x => x.BranchId)
          .HasColumnName("branch_id")
          .IsRequired();

        r.Property(x => x.RoomTypeId)
          .HasColumnName("room_type_id")
          .IsRequired();

        r.Property(x => x.RoomNumber)
          .HasColumnName("room_number")
          .IsRequired()
          .HasMaxLength(20);

        r.Property(x => x.Status)
          .HasColumnName("status")
          .IsRequired()
          .HasConversion<string>()
          .HasMaxLength(20);

        r.Property(x => x.Floor)
          .HasColumnName("floor")
          .IsRequired();

        r.HasIndex(x => new { x.BranchId, x.RoomNumber })
         .IsUnique()
         .HasDatabaseName("UX_rooms_branch_roomnumber");

        r.HasOne(x => x.Branch)
         .WithMany(b => b.Rooms)
         .HasForeignKey(x => x.BranchId)
         .OnDelete(DeleteBehavior.Cascade);

        r.HasOne(x => x.RoomType)
         .WithMany()
         .HasForeignKey(x => x.RoomTypeId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
