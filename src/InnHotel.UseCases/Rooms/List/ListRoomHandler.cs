using InnHotel.Core.RoomAggregate;
using InnHotel.Core.RoomAggregate.Specifications;

namespace InnHotel.UseCases.Rooms.List;

public class ListRoomHandler(IReadRepository<Room> _repository)
    : IQueryHandler<ListRoomQuery, Result<(List<RoomDTO> Items, int TotalCount)>>
{
    public async Task<Result<(List<RoomDTO> Items, int TotalCount)>> Handle(ListRoomQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _repository.CountAsync(cancellationToken);
        
        var spec = new RoomWithDetailsSpec(
            skip: (request.PageNumber - 1) * request.PageSize,
            take: request.PageSize);

        var rooms = await _repository.ListAsync(spec, cancellationToken);

        var roomDtos = rooms.Select(room => new RoomDTO(
            room.Id,
            room.BranchId,
            room.Branch.Name,
            room.RoomTypeId,
            room.RoomType.Name,
            room.RoomType.BasePrice,
            room.RoomType.Capacity,
            room.RoomNumber,
            room.Status,
            room.Floor)).ToList();

        return (roomDtos, totalCount);
    }
}