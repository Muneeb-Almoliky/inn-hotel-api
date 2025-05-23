namespace InnHotel.UseCases.Rooms.Get;

public record GetRoomQuery(int RoomId) : IQuery<Result<RoomDTO>>;