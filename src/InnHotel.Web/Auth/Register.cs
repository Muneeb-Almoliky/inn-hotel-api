using InnHotel.UseCases.Auth.Register;
using Ardalis.Result.AspNetCore;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;
using System.Globalization;
namespace InnHotel.Web.Auth.Register;

/// <summary>
/// [Admin|SuperAdmin] only — register a new user account.
/// </summary>
/// <remarks>
/// Creates a new user with email, password, and optional employee details 
/// (branch, hire date, position). Only users in the Admin or SuperAdmin role 
/// may call this endpoint.
/// </remarks>
public class Register(IMediator _mediator)
    : Endpoint<RegisterRequest, UserCreatedResponse>
{
  public override void Configure()
  {
    Post(RegisterRequest.Route);
    Roles(AuthRoles.Admin, AuthRoles.SuperAdmin);
    Summary(s =>
    {
      s.Summary     = "Register a new user account (Admin/SuperAdmin only)";
      s.Description = "Creates a new ApplicationUser and optional Employee profile.";
      s.ExampleRequest = new RegisterRequest
      {
        Email     = "user@example.com",
        Password  = "SecurePassword123!",
        FirstName = "John",
        LastName  = "Doe",
        BranchId  = 1,
        HireDate = DateOnly.ParseExact("2024-05-01", "yyyy-MM-dd", CultureInfo.InvariantCulture),
        Position = "Receptionist"
      };
    });
  }

  public override async Task HandleAsync(
      RegisterRequest request,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
        new RegisterCommand(
          request.Email!,
          request.Password!,
          request.BranchId,
          request.FirstName,
          request.LastName,
          request.HireDate,
          request.Position
          ),
          cancellationToken
        );

    if (result.IsSuccess)
    {
      Response = new UserCreatedResponse(
        result.Value.Id,
        result.Value.Email,
        $"{request.FirstName} {request.LastName}"
      );
      await SendOkAsync(Response, cancellationToken);
      return;
    }

    await SendResultAsync(result.ToMinimalApiResult());
  }
}
