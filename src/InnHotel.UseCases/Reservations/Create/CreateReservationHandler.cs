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
  private readonly IReadRepository<RoomType> _roomTypeRepo;

  public CreateReservationHandler(
      IRepository<Reservation> reservationRepo,
      IReadRepository<Guest> guestRepo,
      IReadRepository<Room> roomRepo,
      IReadRepository<RoomType> roomTypeRepo)
  {
    _reservationRepo = reservationRepo;
    _guestRepo = guestRepo;
    _roomRepo = roomRepo;
    _roomTypeRepo = roomTypeRepo;
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

    // Get the room's price from the room type
    var roomType = await _roomTypeRepo.GetByIdAsync(room.RoomTypeId, ct);
    if (roomType is null)
      return Result<ReservationDTO>.NotFound("RoomType", "Room type not found.");

    reservation.AddRoom(request.RoomId, roomType.BasePrice);
    await _reservationRepo.SaveChangesAsync(ct);

    var numberOfNights = (request.CheckOutDate.ToDateTime(TimeOnly.MinValue) - request.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
    var totalCost = roomType.BasePrice * numberOfNights;

    var dto = new ReservationDTO(
        Id: created.Id,
        GuestId: guest.Id,
        GuestFullName: $"{guest.FirstName} {guest.LastName}",
        Status: created.Status.ToString(),
        RoomId: room.Id,
        RoomNumber: room.RoomNumber,
        PricePerNight: roomType.BasePrice,
        CheckInDate: created.CheckInDate,
        CheckOutDate: created.CheckOutDate,
        TotalCost: totalCost,
        NumberOfGuests: 1,
        Notes: null
    );

    return Result<ReservationDTO>.Success(dto);
  }
}

