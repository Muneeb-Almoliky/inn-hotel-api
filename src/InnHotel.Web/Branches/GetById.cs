using InnHotel.UseCases.Branches.Get;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Branches;

/// <summary>
/// Get a Branch by integer ID.
/// </summary>
/// <remarks>
/// Takes a positive integer ID and returns a matching Branch record.
/// </remarks>
public class GetById(IMediator _mediator)
    : Endpoint<GetBranchByIdRequest, object>
{
    public override void Configure()
    {
        Get(GetBranchByIdRequest.Route);
        Roles(AuthRoles.SuperAdmin);
  }

  public override async Task HandleAsync(GetBranchByIdRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetBranchQuery(request.BranchId);

        var result = await _mediator.Send(query, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {
            var error = new FailureResponse(404, $"Branch with ID {request.BranchId} not found");
            await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
            return;
        }

        if (result.IsSuccess)
        {
            Response = new BranchRecord(result.Value.Id, result.Value.Name, result.Value.Location);
            await SendOkAsync(Response, cancellationToken);
            return;
        }

        await SendAsync(new FailureResponse(500, "An unexpected error occurred."), statusCode: 500, cancellation: cancellationToken);
    }
}
