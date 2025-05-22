namespace InnHotel.Core.AuthAggregate;
public class RefreshToken(string token, string userId, DateTime expiresAt, string? deviceInfo=null, string? ipAddress=null) : EntityBase, IAggregateRoot
{
  public string Token { get; set; } = Guard.Against.NullOrEmpty(token, nameof(token));
  public string UserId { get; set; } = Guard.Against.NullOrEmpty(userId, nameof(userId));
  public ApplicationUser User { get; private set; } = null!;
  public bool Revoked { get; set; } = false;
  public DateTime? RevokedAt { get; set; }
  public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
  public DateTime ExpiresAt { get; set; } = expiresAt;
  public string? DeviceInfo { get; private set; } = deviceInfo;
  public string? IpAddress { get; private set; } = ipAddress;
}
