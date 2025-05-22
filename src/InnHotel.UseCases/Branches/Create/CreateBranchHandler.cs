using InnHotel.Core.BranchAggregate;
using InnHotel.Core.BranchAggregate.Specifications;

namespace InnHotel.UseCases.Branches.Create;

public class CreateBranchHandler(IRepository<Branch> _repository)
    : ICommandHandler<CreateBranchCommand, Result<BranchDTO>>
{
    public async Task<Result<BranchDTO>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        // Check if branch with same name and location already exists
        var spec = new BranchByNameAndLocationSpec(request.Name, request.Location);
        var existingBranch = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (existingBranch != null)
        {
            return Result.Error("A branch with the same name and location already exists.");
        }

        var branch = new Branch(request.Name, request.Location);

        await _repository.AddAsync(branch, cancellationToken);

        return new BranchDTO(
            branch.Id,
            branch.Name,
            branch.Location);
    }
}