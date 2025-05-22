using InnHotel.UseCases.Guests.Update;
using InnHotel.Web.Common;

namespace InnHotel.Web.Guests;

/// <summary>
/// Update an existing Guest.
/// </summary>
/// <remarks>
/// Update an existing Guest by providing updated details.
/// </remarks>
public class Update(IMediator _mediator)
    : Endpoint<UpdateGuestRequest, object>
{
    public override void Configure()
    {
        Put(UpdateGuestRequest.Route);
    }

    public override async Task HandleAsync(
        UpdateGuestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateGuestCommand(
            request.GuestId,
            request.FirstName,
            request.LastName,
            request.IdProofType,
            request.IdProofNumber,
            request.Email,
            request.Phone,
            request.Address);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendAsync(
                new InnHotelErrorResponse(404, $"Guest with ID {request.GuestId} not found"),
                statusCode: 404,
                cancellation: cancellationToken);
            return;
        }

        if (result.Status == ResultStatus.Error)
        {
            await SendAsync(
                new InnHotelErrorResponse(400, result.Errors.First()),
                statusCode: 400,
                cancellation: cancellationToken);
            return;
        }

        if (result.IsSuccess)
        {
            var guestRecord = new GuestRecord(
                result.Value.Id,
                result.Value.FirstName,
                result.Value.LastName,
                result.Value.IdProofType,
                result.Value.IdProofNumber,
                result.Value.Email,
                result.Value.Phone,
                result.Value.Address);

            await SendAsync(new { status = 200, message = "Guest updated successfully", data = guestRecord },
                statusCode: 200,
                cancellation: cancellationToken);
            return;
        }

        await SendAsync(
            new InnHotelErrorResponse(500, "An unexpected error occurred."),
            statusCode: 500,
            cancellation: cancellationToken);
    }
}