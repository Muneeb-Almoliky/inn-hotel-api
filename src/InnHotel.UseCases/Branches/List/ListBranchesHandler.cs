using InnHotel.Core.BranchAggregate;

namespace InnHotel.UseCases.Branches.List;

public class ListBranchesHandler(IReadRepository<Branch> _repository)
	: IQueryHandler<ListBranchesQuery, Result<(List<BranchDTO> Items, int TotalCount)>>
{
	public async Task<Result<(List<BranchDTO> Items, int TotalCount)>> Handle(ListBranchesQuery request, CancellationToken cancellationToken)
	{
		var totalCount = await _repository.CountAsync(cancellationToken);
		
		var branches = await _repository.ListAsync(cancellationToken);
		var pagedBranches = branches
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize);

		var branchDtos = pagedBranches.Select(entity => new BranchDTO(
						entity.Id,
						entity.Name,
						entity.Location)).ToList();

		return (branchDtos, totalCount);
	}
}