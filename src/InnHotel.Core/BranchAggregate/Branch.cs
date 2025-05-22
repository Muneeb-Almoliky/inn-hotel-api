using InnHotel.Core.RoomAggregate;
using InnHotel.Core.ServiceAggregate;

namespace InnHotel.Core.BranchAggregate;

public class Branch(string name, string location) : EntityBase, IAggregateRoot
{
    public string Name { get; private set; } = Guard.Against.NullOrEmpty(name, nameof(name));
    public string Location { get; private set; } = Guard.Against.NullOrEmpty(location, nameof(location));

    private readonly List<Room> _rooms = new();
    private readonly List<RoomType> _roomTypes = new();
    private readonly List<Service> _services = new();

    public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();
    public IReadOnlyCollection<RoomType> RoomTypes => _roomTypes.AsReadOnly();
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();

    public void UpdateDetails(
        string name,
        string location)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Location = Guard.Against.NullOrEmpty(location, nameof(location));
    }
}
