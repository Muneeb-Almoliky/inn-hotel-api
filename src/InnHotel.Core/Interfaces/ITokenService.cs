using System.Security.Claims;

namespace InnHotel.Core.Interfaces;
public interface ITokenService
{
  string GenerateAccessToken(IEnumerable<Claim> claims);
  string GenerateRefreshToken();
  ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
