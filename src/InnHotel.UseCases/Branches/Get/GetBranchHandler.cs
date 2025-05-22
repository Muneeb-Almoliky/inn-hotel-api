using InnHotel.Core.BranchAggregate;
using InnHotel.Core.BranchAggregate.Specifications;

namespace InnHotel.UseCases.Branches.Get;

public class GetBranchHandler(IReadRepository<Branch> _repository)
    : IQueryHandler<GetBranchQuery, Result<BranchDTO>>
{
    public async Task<Result<BranchDTO>> Handle(GetBranchQuery request, CancellationToken cancellationToken)
    {
        var spec = new BranchByIdSpec(request.branchId);
        var branch = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

        if (branch == null)
        {
            return Result.NotFound();
        }

        var branchDto = new BranchDTO(
            branch.Id,
            branch.Name,
            branch.Location);

        return branchDto;
    }
}