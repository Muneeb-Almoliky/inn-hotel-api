using InnHotel.UseCases.Branches.List;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Branches;

/// <summary>
/// List all Branches with pagination support.
/// </summary>
/// <remarks>
/// Returns a paginated list of Branch records.
/// </remarks>
public class List(IMediator _mediator)
    : Endpoint<PaginationRequest, object>
{
	public override void Configure()
	{
		Get(ListBranchesRequest.Route);
    Roles(AuthRoles.SuperAdmin);
  }

  public override async Task HandleAsync(PaginationRequest request, CancellationToken cancellationToken)
	{
		var query = new ListBranchesQuery(request.PageNumber, request.PageSize);
		var result = await _mediator.Send(query, cancellationToken);

		if (result.IsSuccess)
		{
			var (items, totalCount) = result.Value;
			var branchRecords = items.Select(b => 
					new BranchRecord(b.Id, b.Name, b.Location))
					.ToList();

			var response = new PagedResponse<BranchRecord>(
					branchRecords, 
					totalCount, 
					request.PageNumber, 
					request.PageSize);

			await SendOkAsync(response, cancellationToken);
			return;
		}

		await SendAsync(
				new FailureResponse(500, "An unexpected error occurred."), 
				statusCode: 500, 
				cancellation: cancellationToken);
	}
}
