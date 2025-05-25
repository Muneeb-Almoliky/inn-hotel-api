using Ardalis.Result.AspNetCore;
using InnHotel.UseCases.Reservations.Create;
using Microsoft.AspNetCore.Authorization;

namespace InnHotel.Web.Reservations;

/// <summary>
/// Endpoint to create a new reservation.
/// </summary>
[Authorize] // You can specify roles here if needed
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
        // You can add roles if needed, e.g. Roles("Admin", "Receptionist");
        Summary(s =>
        {
            s.Summary = "Create a new reservation.";
            s.Description = "Creates a new reservation for a guest if the room is available.";
            s.ExampleRequest = new CreateReservationRequest
            {
                GuestId = 1,
                RoomId = 101,
                NumberOfGuests = 2,
                CheckInDate = DateTime.UtcNow.Date.AddDays(1),
                CheckOutDate = DateTime.UtcNow.Date.AddDays(5),
                TotalPrice = 500.00m,
                Notes = "Prefer non-smoking room"
            };
        });
    }

    public override async Task HandleAsync(CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateReservationCommand(
            request.GuestId,
            request.RoomId,
            request.NumberOfGuests,
            request.CheckInDate,
            request.CheckOutDate,
            request.TotalPrice,
            request.Notes
        ), cancellationToken);

        if (result.IsSuccess)
        {
            var reservation = result.Value;

            var record = new ReservationRecord(
                reservation.Id,
                reservation.GuestId,
                reservation.GuestFullName,
                reservation.RoomId,
                reservation.RoomNumber,
                reservation.NumberOfGuests,
                reservation.CheckInDate,
                reservation.CheckOutDate,
                reservation.TotalPrice,
                reservation.Notes);

            await SendAsync(record, 201, cancellationToken);
            return;
        }

        // Handle errors based on the result status
        switch (result.Status)
        {
            case Ardalis.Result.ResultStatus.NotFound:
                await SendNotFoundAsync(result.Errors.First(), cancellationToken);
                break;
            case Ardalis.Result.ResultStatus.Conflict:
                await SendConflictAsync(result.Errors.First(), cancellationToken);
                break;
            case Ardalis.Result.ResultStatus.Error:
                await SendBadRequestAsync(result.Errors.First(), cancellationToken);
                break;
            default:
                await SendErrorsAsync(result.Errors, cancellationToken);
                break;
        }
    }
}
