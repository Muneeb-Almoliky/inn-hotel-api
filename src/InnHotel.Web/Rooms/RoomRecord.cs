namespace InnHotel.Web.Rooms;

public record RoomRecord(
    int Id,
    int BranchId,
    string BranchName,
    int RoomTypeId,
    string RoomTypeName,
    decimal BasePrice,
    int Capacity,
    string RoomNumber,
    RoomStatus Status,
    int Floor);