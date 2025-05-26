namespace InnHotel.Web.Reservations;

public class ReservationServiceRequest
{
    public int ServiceId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
