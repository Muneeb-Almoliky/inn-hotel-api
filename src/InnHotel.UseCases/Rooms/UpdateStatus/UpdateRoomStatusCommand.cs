namespace InnHotel.UseCases.Rooms.UpdateStatus;

public record UpdateRoomStatusCommand(
    int RoomId,
    RoomStatus Status) : ICommand<Result<RoomDTO>>; 