using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using InnHotel.UseCases.Reservations.Create;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Reservations;

/// <summary>
/// Endpoint to create a new reservation.
/// </summary>
public class Create(IMediator _mediator) : Endpoint<CreateReservationRequest, object>
{
  public override void Configure()
  {
    Post(CreateReservationRequest.Route);
    Roles(AuthRoles.SuperAdmin, AuthRoles.Admin);

    Summary(s =>
    {
      s.Summary = "Create a new reservation.";
      s.Description = "Creates a new reservation for a guest if the room is available.";
      s.ExampleRequest = new CreateReservationRequest
      {
        GuestId = 1,
        RoomId = 101,
        CheckInDate = DateTime.UtcNow.Date.AddDays(1),
        CheckOutDate = DateTime.UtcNow.Date.AddDays(5)
      };
    });
  }

  public override async Task HandleAsync(CreateReservationRequest request, CancellationToken cancellationToken)
  {
    var command = new CreateReservationCommand(
        request.GuestId,
        request.RoomId,
        DateOnly.FromDateTime(request.CheckInDate),
        DateOnly.FromDateTime(request.CheckOutDate)
    );

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      var errorMessage = result.Errors.FirstOrDefault() ?? "Resource not found";
      if (errorMessage.Contains("Guest"))
      {
        await SendAsync(
            new FailureResponse(
                404,
                "Guest not found",
                new List<string> { $"No guest exists with ID {request.GuestId}" }
            ),
            statusCode: 404,
            cancellation: cancellationToken);
      }
      else if (errorMessage.Contains("Room"))
      {
        await SendAsync(
            new FailureResponse(
                404,
                "Room not found",
                new List<string> { $"No room exists with ID {request.RoomId}" }
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
                    $"Room {request.RoomId} is already booked from {request.CheckInDate:MMM dd, yyyy} to {request.CheckOutDate:MMM dd, yyyy}",
                    "Please select different dates or choose another room"
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
          reservation.RoomId,
          reservation.RoomNumber,
          reservation.PricePerNight,
          reservation.CheckInDate,
          reservation.CheckOutDate,
          reservation.TotalCost,
          reservation.NumberOfGuests,
          reservation.Notes
      );

      await SendAsync(
          new 
          { 
              status = 201, 
              message = $"Reservation created successfully for guest {reservation.GuestFullName} in room {reservation.RoomNumber}", 
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
