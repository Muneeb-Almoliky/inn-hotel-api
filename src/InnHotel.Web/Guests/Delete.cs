
using InnHotel.UseCases.Guests.Delete;

namespace InnHotel.Web.Guests;

/// <summary>
/// Delete a Guest.
/// </summary>
/// <remarks>
/// Delete a Guest by providing a valid integer id.
/// </remarks>
public class Delete(IMediator _mediator)
  : Endpoint<DeleteGuestRequest>
{
  public override void Configure()
  {
    Delete(DeleteGuestRequest.Route);
  }

  public override async Task HandleAsync(
    DeleteGuestRequest request,
    CancellationToken cancellationToken)
  {
    var command = new DeleteGuestCommand(request.GuestId);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(cancellationToken);
    }
    ;
    // TODO: Handle other issues as needed
  }
}
