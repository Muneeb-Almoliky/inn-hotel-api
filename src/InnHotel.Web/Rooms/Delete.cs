using InnHotel.UseCases.Rooms.Delete;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Rooms;

/// <summary>
/// Delete a Room.
/// </summary>
/// <remarks>
/// Delete a Room by providing a valid integer id.
/// </remarks>
public class Delete(IMediator _mediator)
  : Endpoint<DeleteRoomRequest>
{
  public override void Configure()
  {
    Delete(DeleteRoomRequest.Route);
    Roles(AuthRoles.SuperAdmin, AuthRoles.Admin);
  }

  public override async Task HandleAsync(
    DeleteRoomRequest request,
    CancellationToken cancellationToken)
  {
    var command = new DeleteRoomCommand(request.RoomId);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      var error = new FailureResponse(404, $"Room with ID {request.RoomId} not found");
      await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendAsync(new { status = 200, message = $"Room with ID {request.RoomId} was successfully deleted" }, 
        statusCode: 200, 
        cancellation: cancellationToken);
      return;
    }

    await SendAsync(new FailureResponse(500, "An unexpected error occurred."), statusCode: 500, cancellation: cancellationToken);
  }
}
