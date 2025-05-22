namespace InnHotel.Web.Guests;

public record DeleteGuestRequest
{
  public const string Route = "api/Guests/{GuestId:int}";
  public static string BuildRoute(int guestId) => Route.Replace("{GuestId:int}", guestId.ToString());

  public int GuestId { get; set; }
}

