namespace InnHotel.UseCases.Auth;
public class AuthResult(string accessToken, string refreshToken, string? email, List<string>? roles)
{
  public string AccessToken { get; set; } = accessToken;
  public string RefreshToken { get; set; } = refreshToken;
  public string Email { get; set; } = email ?? string.Empty;
  public List<string> Roles { get; set; } = roles ?? new List<string>();
}
