using InnHotel.Core.RoomAggregate;
using InnHotel.Core.RoomAggregate.Specifications;

namespace InnHotel.UseCases.Rooms.UpdateStatus;

public class UpdateRoomStatusHandler(IRepository<Room> _repository)
    : ICommandHandler<UpdateRoomStatusCommand, Result<RoomDTO>>
{
    public async Task<Result<RoomDTO>> Handle(UpdateRoomStatusCommand request, CancellationToken cancellationToken)
    {
        var spec = new RoomByIdWithDetailsSpec(request.RoomId);
        var room = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (room == null)
            return Result.NotFound();

        room.UpdateStatus(request.Status);
        await _repository.UpdateAsync(room, cancellationToken);

        return new RoomDTO(
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
    }
} 