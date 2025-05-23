namespace InnHotel.Core.Services;
using InnHotel.Core.BranchAggregate.Events;
using InnHotel.Core.BranchAggregate;

using InnHotel.Core.Interfaces;


/// <summary>
/// This is here mainly so there's an example of a domain service
/// and also to demonstrate how to fire domain events from a service.
/// </summary>
/// <param name="_repository"></param>
/// <param name="_mediator"></param>
/// <param name="_logger"></param>
public class DeleteBranchService(IRepository<Branch> _repository,
  IMediator _mediator,
  ILogger<DeleteBranchService> _logger) : IDeleteBranchService
{
  public async Task<Result> DeleteBranch(int branchId)
  {
    _logger.LogInformation("Deleting Contributor {contributorId}", branchId);
    Branch? aggregateToDelete = await _repository.GetByIdAsync(branchId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    var domainEvent = new BranchDeletedEvent(branchId);
    await _mediator.Publish(domainEvent);

    return Result.Success();
  }
}
