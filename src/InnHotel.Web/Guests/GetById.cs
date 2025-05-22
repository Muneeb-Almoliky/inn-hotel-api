using InnHotel.UseCases.Guests.Get;
using InnHotel.Web.Common;

namespace InnHotel.Web.Guests;

/// <summary>
/// Get a Guest by integer ID.
/// </summary>
/// <remarks>
/// Takes a positive integer ID and returns a matching Guest record.
/// </remarks>
public class GetById(IMediator _mediator)
  : Endpoint<GetGuestByIdRequest, object>
{
  public override void Configure()
  {
    Get(GetGuestByIdRequest.Route);
  }

  public override async Task HandleAsync(GetGuestByIdRequest request,
    CancellationToken cancellationToken)
  {
    var query = new GetGuestQuery(request.GuestId);

    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      var error = new FailureResponse(404, $"Guest with ID {request.GuestId} not found");
      await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new GuestRecord(result.Value.Id, result.Value.FirstName, result.Value.LastName, result.Value.IdProofType, result.Value.IdProofNumber, result.Value.Email, result.Value.Phone, result.Value.Address);
      await SendOkAsync(Response, cancellationToken);
      return;
    }

    await SendAsync(new FailureResponse(500, "An unexpected error occurred."), statusCode: 500, cancellation: cancellationToken);
  }
}
