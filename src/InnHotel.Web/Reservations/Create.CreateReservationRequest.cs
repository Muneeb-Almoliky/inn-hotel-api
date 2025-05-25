
namespace InnHotel.Web.Reservations;

public class CreateReservationRequest
{
  public static string Route => "/reservations";

  public int GuestId { get; set; }
  public int RoomId { get; set; }
  public DateTime CheckInDate { get; set; }
  public DateTime CheckOutDate { get; set; }

  public int NumberOfGuests { get; set; } = 1;
  public string? Notes { get; set; }
}
