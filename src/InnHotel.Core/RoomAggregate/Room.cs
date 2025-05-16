using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnHotel.Core.BranchAggregate;


namespace InnHotel.Core.RoomAggregate;

public class Room : EntityBase
{
    public Room(int branchId, int roomTypeId, string roomNumber, RoomStatus status, int floor)
    {
        BranchId = Guard.Against.NegativeOrZero(branchId, nameof(branchId));
        RoomTypeId = Guard.Against.NegativeOrZero(roomTypeId, nameof(roomTypeId));
        RoomNumber = Guard.Against.NullOrEmpty(roomNumber, nameof(roomNumber));
        Status = status;
        Floor = Guard.Against.Negative(floor, nameof(floor));
    }

    protected Room() : this(0, 0, string.Empty, default, 0) { }

    public int BranchId { get; }
    public Branch Branch { get; set; } = null!; // Navigation property
    public int RoomTypeId { get; }
    public RoomType RoomType { get; set; } = null!;
    public string RoomNumber { get; }
    public RoomStatus Status { get; private set; }
    public int Floor { get; }

    public void UpdateStatus(RoomStatus newStatus) => Status = newStatus;
}
