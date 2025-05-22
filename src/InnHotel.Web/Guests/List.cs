using InnHotel.UseCases.Guests.List;
using InnHotel.Web.Common;

namespace InnHotel.Web.Guests;

/// <summary>
/// List all Guests.
/// </summary>
/// <remarks>
/// Returns a list of all Guest records.
/// </remarks>
public class List(IMediator _mediator)
    : EndpointWithoutRequest<object>
{
  public override void Configure()
  {
      Get(ListGuestsRequest.Route);
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    var query = new ListGuestsQuery();

    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsSuccess)
    {
      Response = result.Value.Select(g => 
        new GuestRecord(g.Id, g.FirstName, g.LastName, g.IdProofType, g.IdProofNumber, g.Email, g.Phone, g.Address))
        .ToList();
      await SendOkAsync(Response, cancellationToken);
      return;
    }

    await SendAsync(new InnHotelErrorResponse(500, "An unexpected error occurred."), statusCode: 500, cancellation: cancellationToken);
  }
}