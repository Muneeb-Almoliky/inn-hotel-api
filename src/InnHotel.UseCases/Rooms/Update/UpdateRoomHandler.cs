using InnHotel.Core.RoomAggregate;
using InnHotel.Core.RoomAggregate.Specifications;
using InnHotel.Core.BranchAggregate;
using Microsoft.EntityFrameworkCore;

namespace InnHotel.UseCases.Rooms.Update;

public class UpdateRoomHandler(
    IRepository<Room> _repository,
    IReadRepository<RoomType> _roomTypeRepository)
    : ICommandHandler<UpdateRoomCommand, Result<RoomDTO>>
{
    public async Task<Result<RoomDTO>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var spec = new RoomByIdWithDetailsSpec(request.RoomId);
        var room = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (room == null)
            return Result.NotFound();

        // Validate room type exists and belongs to the same branch
        var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId, cancellationToken);
        if (roomType == null)
            return Result.NotFound("Room type not found");
        if (roomType.BranchId != room.BranchId)
            return Result.Error("Room type does not belong to the room's branch");

        // Check if room number is unique in the branch
        var uniqueSpec = new RoomByBranchAndNumberSpec(room.BranchId, request.RoomNumber);
        var existingRoom = await _repository.FirstOrDefaultAsync(uniqueSpec, cancellationToken);
        if (existingRoom != null && existingRoom.Id != request.RoomId)
            return Result.Error("A room with this number already exists in the branch");

        room.UpdateDetails(
            request.RoomTypeId,
            request.RoomNumber,
            request.Status,
            request.Floor);

        try
        {
            await _repository.UpdateAsync(room, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UX_rooms_branch_roomnumber") == true)
        {
            return Result.Error("A room with this number already exists in the branch");
        }

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