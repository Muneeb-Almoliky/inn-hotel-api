using InnHotel.Core.AuthAggregate;

namespace InnHotel.Infrastructure.Data.Config;
public class ApplicationUserConfiguration: IEntityTypeConfiguration<ApplicationUser>
{
  public void Configure(EntityTypeBuilder<ApplicationUser> u)
  {

     u.HasMany(u => u.RefreshTokens)
      .WithOne() 
      .HasForeignKey(rt => rt.UserId)
      .OnDelete(DeleteBehavior.Cascade);

  }
}


