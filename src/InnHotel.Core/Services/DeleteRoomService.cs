namespace InnHotel.Core.Services;
using InnHotel.Core.RoomAggregate.Events;
using InnHotel.Core.RoomAggregate;
using InnHotel.Core.Interfaces;

/// <summary>
/// This is here mainly so there's an example of a domain service
/// and also to demonstrate how to fire domain events from a service.
/// </summary>
/// <param name="_repository"></param>
/// <param name="_mediator"></param>
/// <param name="_logger"></param>
public class DeleteRoomService(IRepository<Room> _repository,
  IMediator _mediator,
  ILogger<DeleteRoomService> _logger) : IDeleteRoomService
{
  public async Task<Result> DeleteRoom(int roomId)
  {
    _logger.LogInformation("Deleting Room {roomId}", roomId);
    Room? aggregateToDelete = await _repository.GetByIdAsync(roomId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    var domainEvent = new RoomDeletedEvent(roomId);
    await _mediator.Publish(domainEvent);

    return Result.Success();
  }
}