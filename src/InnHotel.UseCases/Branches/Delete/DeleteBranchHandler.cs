namespace InnHotel.UseCases.Branches.Delete;
using InnHotel.Core.Interfaces;

public class DeleteBranchHandler(IDeleteBranchService _deleteBranchService)
  : ICommandHandler<DeleteBranchCommand, Result>
{
  public async Task<Result> Handle(DeleteBranchCommand request, CancellationToken cancellationToken) =>
    await _deleteBranchService.DeleteBranch(request.BranchId);
}