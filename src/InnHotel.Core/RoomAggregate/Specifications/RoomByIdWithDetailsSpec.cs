using Ardalis.Specification;

namespace InnHotel.Core.RoomAggregate.Specifications;

public sealed class RoomByIdWithDetailsSpec : Specification<Room>
{
    public RoomByIdWithDetailsSpec(int roomId)
    {
        Query
            .Where(r => r.Id == roomId)
            .Include(r => r.Branch)
            .Include(r => r.RoomType);
    }
}