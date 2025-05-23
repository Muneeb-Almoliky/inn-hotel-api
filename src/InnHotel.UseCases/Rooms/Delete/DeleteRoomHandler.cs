namespace InnHotel.UseCases.Rooms.Delete;
using InnHotel.Core.Interfaces;

public class DeleteRoomHandler(IDeleteRoomService _deleteRoomService)
  : ICommandHandler<DeleteRoomCommand, Result>
{
  public async Task<Result> Handle(DeleteRoomCommand request, CancellationToken cancellationToken) =>
    await _deleteRoomService.DeleteRoom(request.RoomId);
}