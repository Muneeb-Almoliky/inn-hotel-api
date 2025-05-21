using Ardalis.Result.AspNetCore;
using InnHotel.Core.AuthAggregate;
using InnHotel.UseCases.Auth.Login;

namespace InnHotel.Web.Auth;

public class Login(IMediator _mediator)
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
        new LoginCommand(
          req.Email!,
          req.Password!
        ),
        ct
      );
    if (result.IsSuccess)
    {
      Response = new AuthResponse(
        result.Value.AccessToken,
        result.Value.RefreshToken,
        result.Value.Email,
        result.Value.Roles
      );
      return;
    }

    await SendResultAsync(result.ToMinimalApiResult());
  }
}
