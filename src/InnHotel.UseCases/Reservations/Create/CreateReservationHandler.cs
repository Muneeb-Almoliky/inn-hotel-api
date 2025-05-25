using Ardalis.Result;
using Ardalis.SharedKernel;
using InnHotel.Core.GuestAggregate;
using InnHotel.Core.ReservationAggregate;
using InnHotel.Core.ReservationAggregate.Specifications;
using InnHotel.Core.RoomAggregate;

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
    var guest = await _guestRepo.GetByIdAsync(request.GuestId, ct);
    if (guest is null)
      return Result<ReservationDTO>.NotFound("GuestId", "Guest not found.");

    var room = await _roomRepo.GetByIdAsync(request.RoomId, ct);
    if (room is null)
      return Result<ReservationDTO>.NotFound("RoomId", "Room not found.");

    if (request.CheckOutDate <= request.CheckInDate)
      return Result<ReservationDTO>.Invalid(new[]
      {
                new ValidationError("CheckOutDate", "Check-out date must be after check-in date.")
            });

    var spec = new ReservationByRoomAndDateSpec(request.RoomId, request.CheckInDate, request.CheckOutDate);
    if (await _reservationRepo.AnyAsync(spec, ct))
      return Result<ReservationDTO>.Conflict("RoomAvailability", "Room is already booked for these dates.");

    var reservation = new Reservation(
        guestId: request.GuestId,
        checkInDate: request.CheckInDate,
        checkOutDate: request.CheckOutDate,
        status: ReservationStatus.Confirmed
    );

    var created = await _reservationRepo.AddAsync(reservation, ct);
    await _reservationRepo.SaveChangesAsync(ct);

    reservation.AddRoom(request.RoomId, pricePerNight: 0m);
    await _reservationRepo.SaveChangesAsync(ct);


    var dto = new ReservationDTO(
    Id: created.Id,
    GuestId: guest.Id,
    GuestFullName: $"{guest.FirstName} {guest.LastName}",
    Status: created.Status.ToString(),
    RoomId: room.Id,
    RoomNumber: room.RoomNumber,
    PricePerNight: 0m,
    CheckInDate: created.CheckInDate,
    CheckOutDate: created.CheckOutDate,
    TotalCost: 0m,
    NumberOfGuests: 1,
    Notes: null
  );


    return Result<ReservationDTO>.Success(dto);
  }
}
