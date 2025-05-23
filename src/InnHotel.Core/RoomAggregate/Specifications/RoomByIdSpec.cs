using Ardalis.Specification;

namespace InnHotel.Core.RoomAggregate.Specifications;

public sealed class RoomByIdSpec : Specification<Room>
{
    public RoomByIdSpec(int roomId)
    {
        Query
            .Where(room => room.Id == roomId)
            .Include(room => room.Branch)
            .Include(room => room.RoomType);
    }
}