using Ardalis.Result.AspNetCore;
using InnHotel.Core.AuthAggregate;
using InnHotel.UseCases.Auth.Login;
using InnHotel.Web.Helpers;

namespace InnHotel.Web.Auth;

public class Login(IMediator _mediator, IConfiguration _config)
    : Endpoint<LoginRequest, AuthResponse>
{
  public override void Configure()
  {
    Post(LoginRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Authenticate a user account";
      s.Description = "Returns JWT tokens for API access";
      s.ExampleRequest = new LoginRequest
      {
        Email = "user@example.com",
        Password = "SecurePassword123!"
      };
    });
  }

  public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(
      new LoginCommand(req.Email!, req.Password!),
      ct
    );

    if (!result.IsSuccess)
    {
      await SendResultAsync(result.ToMinimalApiResult());
      return;
    }

    var auth = result.Value;


    // Set the refresh token cookie
    var days = _config.GetValue<int>("Jwt:RefreshTokenExpiryDays");
    CookieHelpers.SetRefreshCookie(HttpContext.Response, auth.RefreshToken, days);


    await SendOkAsync(new AuthResponse(
      accessToken: auth.AccessToken,
      email: auth.Email,
      roles: auth.Roles
    ), ct);
  }
}
