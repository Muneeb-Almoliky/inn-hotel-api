using InnHotel.Core.BranchAggregate;
using InnHotel.Core.BranchAggregate.Specifications;
using Microsoft.EntityFrameworkCore;

namespace InnHotel.UseCases.Branches.Update;

public class UpdateBranchHandler(IRepository<Branch> _repository)
    : ICommandHandler<UpdateBranchCommand, Result<BranchDTO>>
{
    public async Task<Result<BranchDTO>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var spec = new BranchByIdSpec(request.BranchId);
        var branch = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (branch == null)
            return Result.NotFound();

        // Check if name and location combination is already used by another branch
        var nameLocationSpec = new BranchByNameAndLocationSpec(request.Name, request.Location);
        var existingBranch = await _repository.FirstOrDefaultAsync(nameLocationSpec, cancellationToken);
        if (existingBranch != null && existingBranch.Id != request.BranchId)
        {
            return Result.Error("A branch with the same name and location already exists.");
        }

        branch.UpdateDetails(
            request.Name,
            request.Location);

        try
        {
            await _repository.UpdateAsync(branch, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UX_branches_name_location") == true)
        {
            return Result.Error("A branch with the same name and location already exists.");
        }

        return new BranchDTO(
            branch.Id,
            branch.Name,
            branch.Location);
    }
}