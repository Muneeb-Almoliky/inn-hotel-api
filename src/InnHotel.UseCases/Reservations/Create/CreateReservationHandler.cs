using Ardalis.Result;
using Ardalis.SharedKernel;
using InnHotel.Core.GuestAggregate;
using InnHotel.Core.ReservationAggregate;
using InnHotel.Core.ReservationAggregate.Specifications;
using InnHotel.Core.RoomAggregate;
using InnHotel.SharedKernel.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace InnHotel.UseCases.Reservations.Create;

public class CreateReservationHandler : ICommandHandler<CreateReservationCommand, Result<ReservationDTO>>
{
  private readonly IRepository<Reservation> _reservationRepo;
  private readonly IReadRepository<Guest> _guestRepo;
  private readonly IReadRepository<Room> _roomRepo;

  public CreateReservationHandler(
      IRepository<Reservation> reservationRepo,
      IReadRepository<Guest> guestRepo,
      IReadRepository<Room> roomRepo)
  {
    _reservationRepo = reservationRepo;
    _guestRepo = guestRepo;
    _roomRepo = roomRepo;
  }

  public async Task<Result<ReservationDTO>> Handle(CreateReservationCommand request, CancellationToken ct)
  {
    // 1. Check if guest exists
    var guest = await _guestRepo.GetByIdAsync(request.GuestId, ct);
    if (guest is null)
      return Result<ReservationDTO>.NotFound("GuestId", "Guest not found.");

    // 2. Check if room exists
    var room = await _roomRepo.GetByIdAsync(request.RoomId, ct);
    if (room is null)
      return Result<ReservationDTO>.NotFound("RoomId", "Room not found.");

    // 3. Validate dates
    if (request.CheckOutDate <= request.CheckInDate)
      return Result<ReservationDTO>.Invalid(new[]
      {
    new ValidationError("CheckOutDate", "Check-out date must be after check-in date.")
});

    // 4. Check for room conflicts
    var spec = new ReservationByRoomAndDateSpec(request.RoomId, request.CheckInDate, request.CheckOutDate);
    if (await _reservationRepo.AnyAsync(spec, ct))
      return Result<ReservationDTO>.Conflict("RoomAvailability", "Room is already booked for these dates.");

    // 5. Create reservation
    var reservation = new Reservation(
        guestId: request.GuestId,
        checkInDate: request.CheckInDate,
        checkOutDate: request.CheckOutDate,
        status: ReservationStatus.Confirmed
    );

    // 6. Save to get ID
    var created = await _reservationRepo.AddAsync(reservation, ct);
    await _reservationRepo.SaveChangesAsync(ct);

    // 7. Link room
    reservation.AddRoom(request.RoomId, pricePerNight: 0m);
    await _reservationRepo.SaveChangesAsync(ct);

    // 8. Map to DTO
    var dto = new ReservationDTO(
        Id: created.Id,
        GuestId: created.GuestId,
        Status: created.Status.ToString(),
        RoomId: request.RoomId,
        PricePerNight: 0m,
        CheckInDate: created.CheckInDate,
        CheckOutDate: created.CheckOutDate,
        TotalCost: created.TotalCost,
        NumberOfGuests: 1,
        Notes: null
    );

    return Result<ReservationDTO>.Success(dto);
  }
}
