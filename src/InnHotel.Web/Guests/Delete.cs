using InnHotel.UseCases.Guests.Delete;
using InnHotel.Web.Common;

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
      var error = new FailureResponse(404, $"Guest with ID {request.GuestId} not found");
      await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendAsync(new { status = 200, message = $"Guest with ID {request.GuestId} was successfully deleted" }, 
        statusCode: 200, 
        cancellation: cancellationToken);
      return;
    }

    await SendAsync(new FailureResponse(500, "An unexpected error occurred."), statusCode: 500, cancellation: cancellationToken);
  }
}
