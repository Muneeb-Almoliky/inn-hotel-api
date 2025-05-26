using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InnHotel.Web.Guests;

/// <summary>
/// Request DTO to create a new Guest
/// </summary>
public class CreateGuestRequest
{
  public const string Route = "/guests";

  [Required]
  [MaxLength(50)]
  public string? FirstName { get; set; }

  [Required]
  [MaxLength(50)]
  public string? LastName { get; set; }

  [Required]
  [MaxLength(50)]
  public string? IdProofType { get; set; }

  [Required]
  [MaxLength(50)]
  public string? IdProofNumber { get; set; }

  [EmailAddress]
  [MaxLength(100)]
  public string? Email { get; set; }

  [MaxLength(20)]
  public string? Phone { get; set; }

  public string? Address { get; set; }
}

