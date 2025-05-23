using InnHotel.Core.RoomAggregate;
using InnHotel.Core.RoomAggregate.Specifications;
using InnHotel.Core.BranchAggregate;
using InnHotel.Core.BranchAggregate.Specifications;

namespace InnHotel.UseCases.Rooms.Create;

public class CreateRoomHandler(
    IRepository<Room> _roomRepository,
    IReadRepository<Branch> _branchRepository,
    IReadRepository<RoomType> _roomTypeRepository)
    : ICommandHandler<CreateRoomCommand, Result<RoomDTO>>
{
    public async Task<Result<RoomDTO>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        // Validate branch exists
        var branch = await _branchRepository.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch == null)
            return Result.NotFound("Branch not found");

        // Validate room type exists and belongs to the branch
        var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId, cancellationToken);
        if (roomType == null)
            return Result.NotFound("Room type not found");
        if (roomType.BranchId != request.BranchId)
            return Result.Error("Room type does not belong to the specified branch");

        // Check if room number is unique in the branch
        var spec = new RoomByBranchAndNumberSpec(request.BranchId, request.RoomNumber);
        var existingRoom = await _roomRepository.FirstOrDefaultAsync(spec, cancellationToken);
        if (existingRoom != null)
            return Result.Error("A room with this number already exists in the branch");

        var room = new Room(
            request.BranchId,
            request.RoomTypeId,
            request.RoomNumber,
            request.Status,
            request.Floor);

        await _roomRepository.AddAsync(room, cancellationToken);

        return new RoomDTO(
            room.Id,
            room.BranchId,
            branch.Name,
            room.RoomTypeId,
            roomType.Name,
            roomType.BasePrice,
            roomType.Capacity,
            room.RoomNumber,
            room.Status,
            room.Floor);
    }
}