namespace InnHotel.Core.RoomAggregate.Events;

/// <summary>
/// A domain event that is dispatched whenever a room is deleted.
/// The DeleteRoomService is used to dispatch this event.
/// </summary>
internal sealed class RoomDeletedEvent(int roomId) : DomainEventBase
{
  public int roomId { get; init; } = roomId;
}