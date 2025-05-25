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
      SameSite = SameSiteMode.None,
      Expires = DateTime.UtcNow.AddDays(expiryDays),
      Path = "/api/auth"
    });
  }

  public static void ClearRefreshCookie(HttpResponse response)
  {
    response.Cookies.Delete("refreshToken", new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.None,
      Path = "/api/auth"
    });
  }
}
