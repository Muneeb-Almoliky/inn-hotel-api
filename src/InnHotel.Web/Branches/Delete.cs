using InnHotel.UseCases.Branches.Delete;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Branches;

/// <summary>
/// Delete a Branch.
/// </summary>
/// <remarks>
/// Delete a Branch by providing a valid integer id.
/// </remarks>
public class Delete(IMediator _mediator)
  : Endpoint<DeleteBranchRequest>
{
  public override void Configure()
  {
    Delete(DeleteBranchRequest.Route);
    Roles(AuthRoles.SuperAdmin);
  }

  public override async Task HandleAsync(
    DeleteBranchRequest request,
    CancellationToken cancellationToken)
  {
    var command = new DeleteBranchCommand(request.BranchId);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      var error = new FailureResponse(404, $"Branch with ID {request.BranchId} not found");
      await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendAsync(new { status = 200, message = $"Branch with ID {request.BranchId} was successfully deleted" }, 
        statusCode: 200, 
        cancellation: cancellationToken);
      return;
    }

    await SendAsync(new FailureResponse(500, "An unexpected error occurred."), statusCode: 500, cancellation: cancellationToken);
  }
}
