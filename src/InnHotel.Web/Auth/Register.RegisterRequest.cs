using System.ComponentModel.DataAnnotations;
using InnHotel.Core.AuthAggregate;

namespace InnHotel.Web.Auth.Register;

public class RegisterRequest : IRequest<Result<AuthResponse>>
{
  public const string Route = "/auth/register";

  [Required]
  [EmailAddress]
  public string? Email { get; set; }

  [Required]
  [DataType(DataType.Password)]
  [MinLength(8)]
  public string? Password { get; set; }

  [Required]
  [MaxLength(50)]
  public string? FirstName { get; set; }

  [Required]
  [MaxLength(50)]
  public string? LastName { get; set; }

  [Required]
  public int? BranchId { get; set; }

  [Required]
  public DateOnly? HireDate { get; set; }

  [Required]
  [MaxLength(100)]
  public string? Position { get; set; }
}
