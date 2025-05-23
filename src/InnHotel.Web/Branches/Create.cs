using InnHotel.UseCases.Branches.Create;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Branches;

/// <summary>
/// Create a new Branch.
/// </summary>
/// <remarks>
/// Creates a new Branch with the provided details.
/// </remarks>
public class Create(IMediator _mediator)
    : Endpoint<CreateBranchRequest, object>
{
    public override void Configure()
    {
        Post(CreateBranchRequest.Route);
        Roles(AuthRoles.SuperAdmin);
    }

    public override async Task HandleAsync(
        CreateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBranchCommand(
            request.Name,
            request.Location);

        var result = await _mediator.Send(command, cancellationToken);

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

            await SendAsync(
                new { status = 201, message = "Branch created successfully", data = branchRecord },
                statusCode: 201,
                cancellation: cancellationToken);
            return;
        }

        await SendAsync(
            new FailureResponse(500, "An unexpected error occurred."),
            statusCode: 500,
            cancellation: cancellationToken);
    }
}