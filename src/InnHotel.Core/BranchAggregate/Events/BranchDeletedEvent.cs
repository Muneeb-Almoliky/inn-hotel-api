namespace InnHotel.Core.BranchAggregate.Events;

/// <summary>
/// A domain event that is dispatched whenever a branch is deleted.
/// The DeleteBranchService is used to dispatch this event.
/// </summary>
internal sealed class BranchDeletedEvent(int branchId) : DomainEventBase
{
  public int branchId { get; init; } = branchId;
}
