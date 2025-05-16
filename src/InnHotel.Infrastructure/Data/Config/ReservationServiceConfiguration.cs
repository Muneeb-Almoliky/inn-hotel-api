using InnHotel.Core.ReservationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnHotel.Infrastructure.Data.Config;

public class ReservationServiceConfiguration : IEntityTypeConfiguration<ReservationService>
{
    public void Configure(EntityTypeBuilder<ReservationService> rs)
    {
        rs.ToTable("reservation_services", t =>
        {
            t.HasCheckConstraint("CK_res_services_quantity", "quantity > 0");
            t.HasCheckConstraint("CK_res_services_totalprice", "total_price >= 0");
        });

        rs.HasKey(x => x.Id).HasName("reservation_service_id");

        rs.Property(x => x.ReservationId)
          .HasColumnName("reservation_id")
          .IsRequired();
        rs.Property(x => x.ServiceId)
          .HasColumnName("service_id")
          .IsRequired();

        rs.Property(x => x.Quantity)
          .HasColumnName("quantity")
          .IsRequired();

        rs.Property(x => x.TotalPrice)
          .HasColumnName("total_price")
          .HasColumnType("numeric(10,2)")
          .IsRequired();

        rs.HasOne(x => x.Reservation)
          .WithMany(r => r.Services)
          .HasForeignKey(x => x.ReservationId);

        rs.HasOne(x => x.Service)
          .WithMany()
          .HasForeignKey(x => x.ServiceId)
          .OnDelete(DeleteBehavior.Restrict);
    }
}
