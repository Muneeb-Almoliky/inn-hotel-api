namespace InnHotel.Core.RoomAggregate;

public class RoomService : EntityBase
{
    public RoomService(int roomId, int serviceId)
    {
        RoomId = Guard.Against.NegativeOrZero(roomId, nameof(roomId));
        ServiceId = Guard.Against.NegativeOrZero(serviceId, nameof(serviceId));
    }

    protected RoomService() : this(0, 0) { }

    public int RoomId { get; }
    public int ServiceId { get; }
}
