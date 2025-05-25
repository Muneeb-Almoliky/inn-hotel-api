using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using InnHotel.UseCases.Reservations.Create;
using Microsoft.AspNetCore.Authorization;

namespace InnHotel.Web.Reservations;

/// <summary>
/// Endpoint to create a new reservation.
/// </summary>
[Authorize]
public class Create : Endpoint<CreateReservationRequest, ReservationRecord>
{
  private readonly IMediator _mediator;

  public Create(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post(CreateReservationRequest.Route);

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

      await SendAsync(record, 201, cancellationToken);
      return;
    }

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.Status == ResultStatus.Conflict)
    {
      await SendErrorsAsync( 409, cancellationToken);
      return;
    }

    // لباقي الأخطاء
    await SendErrorsAsync(400, cancellationToken);
  }
}
