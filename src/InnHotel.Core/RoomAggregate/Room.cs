using InnHotel.Core.BranchAggregate;
using Ardalis.SharedKernel;

namespace InnHotel.Core.RoomAggregate;

public class Room(
    int branchId,
    int roomTypeId,
    string roomNumber,
    RoomStatus status,
    int floor
) : EntityBase, IAggregateRoot
{
    public int BranchId { get; private set; }
        = Guard.Against.NegativeOrZero(branchId, nameof(branchId));   

    public int RoomTypeId { get; private set; }
        = Guard.Against.NegativeOrZero(roomTypeId, nameof(roomTypeId)); 

    public string RoomNumber { get; private set; }
        = Guard.Against.NullOrEmpty(roomNumber, nameof(roomNumber));   

    public RoomStatus Status { get; private set; }
        = status;                                                    

    public int Floor { get; private set; }
        = Guard.Against.Negative(floor, nameof(floor));                 

    public Branch Branch { get; set; } = null!;      
    public RoomType RoomType { get; set; } = null!;   

    public void UpdateStatus(RoomStatus newStatus)
        => Status = newStatus;                           

    public void UpdateDetails(
        int roomTypeId,
        string roomNumber,
        RoomStatus status,
        int floor)
    {
        RoomTypeId = Guard.Against.NegativeOrZero(roomTypeId, nameof(roomTypeId));
        RoomNumber = Guard.Against.NullOrEmpty(roomNumber, nameof(roomNumber));
        Status = status;
        Floor = Guard.Against.Negative(floor, nameof(floor));
    }
}

