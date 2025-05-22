namespace InnHotel.Core.Services;
using InnHotel.Core.GuestAggregate.Events;
using InnHotel.Core.GuestAggregate;

using InnHotel.Core.Interfaces;


/// <summary>
/// This is here mainly so there's an example of a domain service
/// and also to demonstrate how to fire domain events from a service.
/// </summary>
/// <param name="_repository"></param>
/// <param name="_mediator"></param>
/// <param name="_logger"></param>
public class DeleteGuestService(IRepository<Guest> _repository,
  IMediator _mediator,
  ILogger<DeleteGuestService> _logger) : IDeleteGuestService
{
  public async Task<Result> DeleteGuest(int guestId)
  {
    _logger.LogInformation("Deleting Contributor {contributorId}", guestId);
    Guest? aggregateToDelete = await _repository.GetByIdAsync(guestId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    var domainEvent = new GuestDeletedEvent(guestId);
    await _mediator.Publish(domainEvent);

    return Result.Success();
  }
}
