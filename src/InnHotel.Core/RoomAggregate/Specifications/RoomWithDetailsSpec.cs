using Ardalis.Specification;

namespace InnHotel.Core.RoomAggregate.Specifications;

public sealed class RoomWithDetailsSpec : Specification<Room>
{
    public RoomWithDetailsSpec(int skip, int take)
    {
        Query
            .Include(r => r.Branch)
            .Include(r => r.RoomType)
            .Skip(skip)
            .Take(take);
    }
}