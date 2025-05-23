using Ardalis.Specification;

namespace InnHotel.Core.RoomAggregate.Specifications;

public sealed class RoomByBranchAndNumberSpec : Specification<Room>
{
    public RoomByBranchAndNumberSpec(int branchId, string roomNumber)
    {
        Query
            .Where(r => r.BranchId == branchId && r.RoomNumber == roomNumber);
    }
}