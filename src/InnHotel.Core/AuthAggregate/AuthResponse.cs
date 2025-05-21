using System.ComponentModel.DataAnnotations;

namespace InnHotel.Core.AuthAggregate;

public class AuthResponse(string accessToken, string refreshToken)
{
  public string AccessToken { get; set; } = accessToken;
  public string RefreshToken { get; set; } = refreshToken;
}


