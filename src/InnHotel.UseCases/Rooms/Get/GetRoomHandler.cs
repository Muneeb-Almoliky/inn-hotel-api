using InnHotel.Core.RoomAggregate;
using InnHotel.Core.RoomAggregate.Specifications;

namespace InnHotel.UseCases.Rooms.Get;

public class GetRoomHandler(IReadRepository<Room> _repository)
    : IQueryHandler<GetRoomQuery, Result<RoomDTO>>
{
    public async Task<Result<RoomDTO>> Handle(GetRoomQuery request, CancellationToken cancellationToken)
    {
        var spec = new RoomByIdSpec(request.RoomId);
        var room = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

        if (room == null)
            return Result.NotFound();

        var roomDto = new RoomDTO(
            room.Id,
            room.BranchId,
            room.Branch.Name,
            room.RoomTypeId,
            room.RoomType.Name,
            room.RoomType.BasePrice,
            room.RoomType.Capacity,
            room.RoomNumber,
            room.Status,
            room.Floor);

        return roomDto;
    }
}