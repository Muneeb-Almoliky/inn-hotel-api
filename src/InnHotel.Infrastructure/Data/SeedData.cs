using InnHotel.Core.BranchAggregate;
using InnHotel.Core.GuestAggregate;
using InnHotel.Core.RoomAggregate;
using InnHotel.Core.ReservationAggregate;

namespace InnHotel.Infrastructure.Data;

public static class SeedData
{
  public static readonly Branch MainBranch = new("Main Hotel", "123 Seaside Boulevard");
  public static readonly Branch DowntownBranch = new("Downtown Suites", "456 City Center Road");

  public static readonly RoomType StandardRoom = new(
      branchId: 1,
      name: "Standard",
      basePrice: 99.99m,
      capacity: 2,
      description: "Comfortable standard room with queen bed");

  public static readonly RoomType Suite = new(
      branchId: 1,
      name: "Suite",
      basePrice: 199.99m,
      capacity: 4,
      description: "Luxurious suite with king bed and ocean view");

  public static readonly Guest TestGuest = new(
      "John",
      "Doe",
      "Passport",
      "AB1234567",
      email: "john.doe@example.com",
      phone: "+1234567890",
      address: "123 Main Street");

  public static async Task InitializeAsync(AppDbContext dbContext)
  {
    if (await dbContext.Branches.AnyAsync()) return; // Already seeded

    await PopulateTestDataAsync(dbContext);
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext)
  {
    // Add Branches
    dbContext.Branches.AddRange(MainBranch, DowntownBranch);
    await dbContext.SaveChangesAsync();

    // Add Room Types
    dbContext.RoomTypes.AddRange(StandardRoom, Suite);
    await dbContext.SaveChangesAsync();

    // Add Rooms
    var rooms = new List<Room>
        {
            new(MainBranch.Id, StandardRoom.Id, "101", RoomStatus.Available, 1),
            new(MainBranch.Id, StandardRoom.Id, "102", RoomStatus.Available, 1),
            new(MainBranch.Id, Suite.Id, "201", RoomStatus.Available, 2)
        };
    dbContext.Rooms.AddRange(rooms);

    // Add Guest
    dbContext.Guests.Add(TestGuest);

    await dbContext.SaveChangesAsync();

    // Add Sample Reservation
    var reservation = new Reservation(
        TestGuest.Id,
        DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
        DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
        ReservationStatus.Confirmed,
        1399.93m
    );

    reservation.AddRoom(rooms[0].Id, 99.99m);
    reservation.AddService(1, 2, 50.00m);

    dbContext.Reservations.Add(reservation);
    await dbContext.SaveChangesAsync();
  }
}
