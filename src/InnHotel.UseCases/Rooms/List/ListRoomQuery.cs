namespace InnHotel.UseCases.Rooms.List;

public record ListRoomQuery(int PageNumber, int PageSize) 
    : IQuery<Result<(List<RoomDTO> Items, int TotalCount)>>;