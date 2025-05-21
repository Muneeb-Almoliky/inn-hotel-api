using InnHotel.Core.AuthAggregate;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
  public void Configure(EntityTypeBuilder<RefreshToken> builder)
  {
    builder.HasKey(rt => rt.Id);
    builder.Property(rt => rt.Token)
        .IsRequired()
        .HasMaxLength(1024);

    builder.HasOne(rt => rt.User)
        .WithMany(e => e.RefreshTokens)
        .HasForeignKey(rt => rt.UserId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
