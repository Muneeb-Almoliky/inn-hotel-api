using Ardalis.Result.AspNetCore;
using InnHotel.UseCases.Guests.Create;

namespace InnHotel.Web.Guests;

/// <summary>
/// Create a new Guest
/// </summary>
public class Create(IMediator _mediator)
  : Endpoint<CreateGuestRequest, CreateGuestResponse>
{
  public override void Configure()
  {
    Post(CreateGuestRequest.Route);
    Summary(s =>
    {
      // XML Docs are used by default but are overridden by these properties:
      // s.Summary = "Create a new Guest.";
      // s.Description = "Provide required ID proofs and optional contact info.";
      s.ExampleRequest = new CreateGuestRequest
      {
        FirstName = "Jane",
        LastName = "Doe",
        IdProofType = "Passport",
        IdProofNumber = "X1234567",
        Email = "jane.doe@example.com",
        Phone = "+1234567890",
        Address = "123 Main St"
      };
    });
  }

  public override async Task HandleAsync(
    CreateGuestRequest request,
    CancellationToken cancellationToken)
  {
    // Invoke the CreateGuestCommand via MediatR
    var result = await _mediator.Send(
      new CreateGuestCommand(
        request.FirstName!,
        request.LastName!,
        request.IdProofType!,
        request.IdProofNumber!,
        request.Email,
        request.Phone,
        request.Address
      ),
      cancellationToken
    );

    if (result.IsSuccess)
    {
      Response = new CreateGuestResponse(
        result.Value,
        request.FirstName!,
        request.LastName!,
        request.IdProofType!,
        request.IdProofNumber!,
        request.Email,
        request.Phone,
        request.Address
      );
      return;
    }

    // TODO: handle failure cases (e.g. validation errors)
    await SendResultAsync(result.ToMinimalApiResult());
  }
}
