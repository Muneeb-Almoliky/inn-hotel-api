namespace InnHotel.UseCases.Rooms.Delete;

public record DeleteRoomCommand(int RoomId) : ICommand<Result>;