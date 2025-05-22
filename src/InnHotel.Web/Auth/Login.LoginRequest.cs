using System.ComponentModel.DataAnnotations;

namespace InnHotel.Web.Auth;

public class LoginRequest
{
  public const string Route = "/auth/login";

  [Required]
  [EmailAddress]
  public string? Email { get; set; }

  [Required]
  [DataType(DataType.Password)]
  public string? Password { get; set; }
}
