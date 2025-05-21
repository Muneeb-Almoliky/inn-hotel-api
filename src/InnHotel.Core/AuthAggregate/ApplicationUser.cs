using InnHotel.Core.EmployeeAggregate;
using Microsoft.AspNetCore.Identity;

namespace InnHotel.Core.AuthAggregate;

public class ApplicationUser : IdentityUser, IAggregateRoot
{
  public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
