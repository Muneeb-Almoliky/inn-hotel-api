using Ardalis.Result.AspNetCore;
using InnHotel.Core.AuthAggregate;
using InnHotel.UseCases.Auth.Logout;
using InnHotel.Web.Helpers;
using System.Security.Claims;

namespace InnHotel.Web.Auth;

public class Logout(IMediator _mediator) : Endpoint<LogoutRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post(LogoutRequest.Route);
        Summary(s =>
        {
            s.Summary = "Logout a user";
            s.Description = "Invalidates the refresh token and clears the cookie";
            s.ExampleRequest = new LogoutRequest
            {
                RefreshToken = "your-refresh-token"
            };
        });
    }

    public override async Task HandleAsync(LogoutRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _mediator.Send(
            new LogoutCommand(userId, req.RefreshToken!),
            ct
        );

        if (!result.IsSuccess)
        {
            await SendResultAsync(result.ToMinimalApiResult());
            return;
        }

        // Clear the refresh token cookie
        CookieHelpers.ClearRefreshCookie(HttpContext.Response);

        await SendNoContentAsync(ct);
    }
}