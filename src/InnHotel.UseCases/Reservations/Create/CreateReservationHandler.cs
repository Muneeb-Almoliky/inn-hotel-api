using Ardalis.Result;
using Ardalis.SharedKernel;
using InnHotel.Core.GuestAggregate;
using InnHotel.Core.ReservationAggregate;
using InnHotel.Core.ReservationAggregate.Specifications;
using InnHotel.Core.RoomAggregate;
using InnHotel.Core.ServiceAggregate;

namespace InnHotel.UseCases.Reservations.Create;

public class CreateReservationHandler(IRepository<Reservation> _reservationRepo,
      IReadRepository<Guest> _guestRepo,
      IReadRepository<Room> _roomRepo,
      IReadRepository<Service> _serviceRepo)
  : ICommandHandler<CreateReservationCommand, Result<ReservationDTO>>
{

 
  public async Task<Result<ReservationDTO>> Handle(
    CreateReservationCommand request,
    CancellationToken ct)
  {
    // Check if guest exists
    var guest = await _guestRepo.GetByIdAsync(request.GuestId, ct);
    if (guest == null)
    {
      return Result<ReservationDTO>.NotFound($"Guest with ID {request.GuestId} not found");
    }

    // Validate rooms
    foreach (var room in request.Rooms)
    {
      var roomEntity = await _roomRepo.GetByIdAsync(room.RoomId, ct);
      if (roomEntity == null)
      {
        return Result<ReservationDTO>.NotFound($"Room with ID {room.RoomId} not found");
      }

      // Check if room is available for the requested dates
      var roomAvailabilitySpec = new RoomAvailabilitySpec(room.RoomId, request.CheckInDate, request.CheckOutDate);
      var existingReservation = await _reservationRepo.FirstOrDefaultAsync(roomAvailabilitySpec, ct);
      if (existingReservation != null)
      {
        return Result<ReservationDTO>.Conflict(
          $"Room {roomEntity.RoomNumber} is already booked for the selected dates"
        );
      }
    }

    // Validate services
    foreach (var service in request.Services)
    {
      var serviceEntity = await _serviceRepo.GetByIdAsync(service.ServiceId, ct);
      if (serviceEntity == null)
      {
        return Result<ReservationDTO>.NotFound($"Service with ID {service.ServiceId} not found");
      }
    }

    // Create new reservation
    var reservation = new Reservation(
      request.GuestId,
      request.CheckInDate,
      request.CheckOutDate,
      request.Status
    );

    // Add rooms to reservation
    foreach (var room in request.Rooms)
    {
      reservation.AddRoom(room.RoomId, room.PricePerNight);
    }

    // Add services to reservation
    foreach (var service in request.Services)
    {
      reservation.AddService(service.ServiceId, service.Quantity, service.UnitPrice);
    }

    // Save reservation
    await _reservationRepo.AddAsync(reservation, ct);
    await _reservationRepo.SaveChangesAsync(ct);

        var dto = new ReservationDTO(
          reservation.Id,
          reservation.GuestId,
          request.GuestFullName,
          reservation.CheckInDate,
          reservation.CheckOutDate,
          reservation.Status,
          reservation.TotalCost,
          reservation.Rooms
            .Select(r => new ReservationRoomDTO(r.RoomId, r.PricePerNight))
            .ToList(),
          reservation.Services
            .Select(s => new ReservationServiceDTO(s.ServiceId, s.Quantity, s.TotalPrice))
            .ToList()
        );

    return Result<ReservationDTO>.Success(dto);
  }
}
