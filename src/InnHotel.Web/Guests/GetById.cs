using InnHotel.UseCases.Guests.Get;

namespace InnHotel.Web.Guests;

/// <summary>
/// Get a Guest by integer ID.
/// </summary>
/// <remarks>
/// Takes a positive integer ID and returns a matching Guest record.
/// </remarks>
public class GetById(IMediator _mediator)
  : Endpoint<GetGuestByIdRequest, GuestRecord>
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
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new GuestRecord(result.Value.Id, result.Value.FirstName, result.Value.LastName, result.Value.IdProofType, result.Value.IdProofNumber, result.Value.Email, result.Value.Phone, result.Value.Address);
    }
  }
}
