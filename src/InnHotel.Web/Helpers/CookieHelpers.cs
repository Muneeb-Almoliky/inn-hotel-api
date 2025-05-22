namespace InnHotel.Web.Helpers;

public static class CookieHelpers
{
  public static void SetRefreshCookie(
      HttpResponse response,
      string token,
      int expiryDays)
  {
    response.Cookies.Delete("refreshToken");
    response.Cookies.Append("refreshToken", token, new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = DateTime.UtcNow.AddDays(expiryDays),
      Path = "/"
    });
  }
}
