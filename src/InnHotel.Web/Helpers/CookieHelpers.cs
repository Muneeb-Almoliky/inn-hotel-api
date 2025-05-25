namespace InnHotel.Web.Helpers;

public static class CookieHelpers
{
  public static void SetRefreshCookie(
      HttpResponse response,
      string token,
      int expiryDays)
  {
    response.Cookies.Append("refreshToken", token, new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.None,
      Expires = DateTime.UtcNow.AddDays(expiryDays),
      Path = "/api/auth"
    });
    response.Cookies.Delete("refreshToken");
  }
}
