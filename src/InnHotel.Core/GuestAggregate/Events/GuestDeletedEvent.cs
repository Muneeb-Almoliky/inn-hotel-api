namespace InnHotel.Core.GuestAggregate.Events;

/// <summary>
/// A domain event that is dispatched whenever a guest is deleted.
/// The DeleteGuestService is used to dispatch this event.
/// </summary>
internal sealed class GuestDeletedEvent(int guestId) : DomainEventBase
{
  public int guestId { get; init; } = guestId;
}
