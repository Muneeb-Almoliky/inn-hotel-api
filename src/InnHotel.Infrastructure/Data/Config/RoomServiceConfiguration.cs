using InnHotel.Core.ServiceAggregate;
using InnHotel.Core.RoomAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnHotel.Infrastructure.Data.Config;


public class RoomServiceConfiguration : IEntityTypeConfiguration<RoomService>
{
  public void Configure(EntityTypeBuilder<RoomService> rs)
  {
    rs.ToTable("room_services");
    rs.HasKey(x => new { x.RoomId, x.ServiceId });
  }
}
