using InnHotel.Core.ReservationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnHotel.Infrastructure.Data.Config;

public class ReservationRoomConfiguration : IEntityTypeConfiguration<ReservationRoom>
{
    public void Configure(EntityTypeBuilder<ReservationRoom> rr)
    {
        rr.ToTable("reservation_rooms", t =>
        {
            t.HasCheckConstraint("CK_res_rooms_price", "price_per_night > 0");
        });

        rr.HasKey(x => x.Id).HasName("reservation_room_id");

        rr.Property(x => x.ReservationId)
          .HasColumnName("reservation_id")
          .IsRequired();
        rr.Property(x => x.RoomId)
          .HasColumnName("room_id")
          .IsRequired();

        rr.Property(x => x.PricePerNight)
          .HasColumnName("price_per_night")
          .HasColumnType("numeric(10,2)")
          .IsRequired();

        rr.HasOne(x => x.Reservation)
          .WithMany(r => r.Rooms)
          .HasForeignKey(x => x.ReservationId);

        rr.HasOne(x => x.Room)
          .WithMany()
          .HasForeignKey(x => x.RoomId)
          .OnDelete(DeleteBehavior.Restrict);
    }
}
