using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Ardalis.SharedKernel;
using InnHotel.Core.ReservationAggregate;
using InnHotel.UseCases.Reservations;
using InnHotel.UseCases.Reservations.Create;
using InnHotel.Web.Common;
using InnHotel.Core.GuestAggregate;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Reservations;

/// <summary>
/// Endpoint to create a new reservation.
/// </summary>
public class Create(IMediator _mediator, IRepository<Guest> _guestRepo) : Endpoint<CreateReservationRequest, object>
{
  public override void Configure()
  {
    Post(CreateReservationRequest.Route);
    Roles(AuthRoles.SuperAdmin, AuthRoles.Admin);

    Summary(s =>
    {
      s.Summary = "Create a new reservation.";
      s.Description = "Creates a new reservation for a guest with specified rooms and services.";
      s.ExampleRequest = new CreateReservationRequest
      {
        GuestId = 1,
        CheckInDate = DateTime.UtcNow.Date.AddDays(1),
        CheckOutDate = DateTime.UtcNow.Date.AddDays(5),
        Rooms = new List<ReservationRoomRequest>
        {
          new() { RoomId = 101, PricePerNight = 100.00m }
        },
        Services = new List<ReservationServiceRequest>
        {
          new() { ServiceId = 1, Quantity = 2, UnitPrice = 25.00m }
        }
      };
    });
  }

  public override async Task HandleAsync(CreateReservationRequest request, CancellationToken cancellationToken)
  {
    // Get guest to get full name
    var guest = await _guestRepo.GetByIdAsync(request.GuestId, cancellationToken);
    if (guest == null)
    {
      await SendAsync(
          new FailureResponse(
              404,
              "Guest not found",
              new List<string> { $"No guest exists with ID {request.GuestId}" }
          ),
          statusCode: 404,
          cancellation: cancellationToken);
      return;
    }

    var command = new CreateReservationCommand(
        request.GuestId,
        $"{guest.FirstName} {guest.LastName}",
        DateOnly.FromDateTime(request.CheckInDate),
        DateOnly.FromDateTime(request.CheckOutDate),
        ReservationStatus.Pending,
        request.Rooms.Select(r => new ReservationRoomDTO(r.RoomId, r.PricePerNight)).ToList(),
        request.Services.Select(s => new ReservationServiceDTO(s.ServiceId, s.Quantity, s.UnitPrice)).ToList()
    );

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      var errorMessage = result.Errors.FirstOrDefault() ?? "Resource not found";
      if (errorMessage.Contains("Room"))
      {
        await SendAsync(
            new FailureResponse(
                404,
                "Room not found",
                new List<string> { $"One or more rooms were not found" }
            ),
            statusCode: 404,
            cancellation: cancellationToken);
      }
      else if (errorMessage.Contains("Service"))
      {
        await SendAsync(
            new FailureResponse(
                404,
                "Service not found",
                new List<string> { $"One or more services were not found" }
            ),
            statusCode: 404,
            cancellation: cancellationToken);
      }
      else
      {
        await SendAsync(
            new FailureResponse(
                404,
                "Resource not found",
                new List<string> { errorMessage }
            ),
            statusCode: 404,
            cancellation: cancellationToken);
      }
      return;
    }

    if (result.Status == ResultStatus.Conflict)
    {
      var errorMessage = result.Errors.FirstOrDefault() ?? "Conflict occurred";
      if (errorMessage.Contains("already booked"))
      {
        await SendAsync(
            new FailureResponse(
                409,
                "Room unavailable for selected dates",
                new List<string> 
                { 
                    $"One or more rooms are already booked for the selected dates ({request.CheckInDate:MMM dd, yyyy} to {request.CheckOutDate:MMM dd, yyyy})",
                    "Please select different dates or choose other rooms"
                }
            ),
            statusCode: 409,
            cancellation: cancellationToken);
      }
      else
      {
        await SendAsync(
            new FailureResponse(
                409,
                "Reservation conflict",
                new List<string> { errorMessage }
            ),
            statusCode: 409,
            cancellation: cancellationToken);
      }
      return;
    }

    if (result.IsSuccess)
    {
      var reservation = result.Value;
      var record = new ReservationRecord(
          reservation.Id,
          reservation.GuestId,
          reservation.GuestFullName,
          reservation.Status,
          reservation.Rooms,
          reservation.Services,
          reservation.CheckInDate,
          reservation.CheckOutDate,
          reservation.TotalCost
      );

      await SendAsync(
          new 
          { 
              status = 201, 
              message = $"Reservation created successfully for guest {reservation.GuestFullName}", 
              data = record 
          },
          statusCode: 201,
          cancellation: cancellationToken);
      return;
    }

    // Handle validation errors
    if (result.Errors.Any())
    {
      await SendAsync(
          new FailureResponse(
              400,
              "Invalid reservation request",
              result.Errors.ToList()
          ),
          statusCode: 400,
          cancellation: cancellationToken);
      return;
    }

    // Handle unexpected errors
    await SendAsync(
        new FailureResponse(
            500,
            "Internal server error",
            new List<string> { "An unexpected error occurred while processing your reservation request" }
        ),
        statusCode: 500,
        cancellation: cancellationToken);
  }
}
