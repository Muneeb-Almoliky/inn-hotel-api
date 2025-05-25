namespace InnHotel.Web.Reservations;

public class CreateReservationRequest
{
    public int GuestId { get; set; }
    public int RoomId { get; set; }
    public int NumberOfGuests { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }

    public const string Route = "/reservations";
}
