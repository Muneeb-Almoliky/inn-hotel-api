using System.ComponentModel.DataAnnotations;

namespace InnHotel.Core.AuthAggregate;

public class AuthResponse(string accessToken, string? email, List<string>? roles)
{
  public string AccessToken { get; set; } = accessToken;
  public string Email { get; set; } = email ?? string.Empty;
  public List<string> Roles { get; set; } = roles ?? new List<string>();
}


