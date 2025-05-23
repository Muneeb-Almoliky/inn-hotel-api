using InnHotel.UseCases.Branches.Update;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Branches;

/// <summary>
/// Update an existing Branch.
/// </summary>
/// <remarks>
/// Update an existing Branch by providing updated details.
/// </remarks>
public class Update(IMediator _mediator)
    : Endpoint<UpdateBranchRequest, object>
{
    public override void Configure()
    {
        Put(UpdateBranchRequest.Route);
        Roles(AuthRoles.SuperAdmin);
  }

  public override async Task HandleAsync(
        UpdateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBranchCommand(
            request.BranchId,
            request.Name,
            request.Location);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendAsync(
                new FailureResponse(404, $"Branch with ID {request.BranchId} not found"),
                statusCode: 404,
                cancellation: cancellationToken);
            return;
        }

        if (result.Status == ResultStatus.Error)
        {
            await SendAsync(
                new FailureResponse(400, result.Errors.First()),
                statusCode: 400,
                cancellation: cancellationToken);
            return;
        }

        if (result.IsSuccess)
        {
            var branchRecord = new BranchRecord(
                result.Value.Id,
                result.Value.Name,
                result.Value.Location);

            await SendAsync(new { status = 200, message = "Branch updated successfully", data = branchRecord },
                statusCode: 200,
                cancellation: cancellationToken);
            return;
        }

        await SendAsync(
            new FailureResponse(500, "An unexpected error occurred."),
            statusCode: 500,
            cancellation: cancellationToken);
    }
}
