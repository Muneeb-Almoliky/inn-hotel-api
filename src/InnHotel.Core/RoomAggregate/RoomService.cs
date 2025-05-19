namespace InnHotel.Core.RoomAggregate;

public class RoomService(int roomId, int serviceId) : EntityBase
{
    public int RoomId { get; private set; } = Guard.Against.NegativeOrZero(roomId, nameof(roomId));
    public int ServiceId { get; private set; } = Guard.Against.NegativeOrZero(serviceId, nameof(serviceId));
}
