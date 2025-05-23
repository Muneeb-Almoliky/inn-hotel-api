using InnHotel.UseCases.Rooms.UpdateStatus;
using InnHotel.Web.Common;
using InnHotel.Core.RoomAggregate;

namespace InnHotel.Web.Rooms;

/// <summary>
/// Update a room's status.
/// </summary>
/// <remarks>
/// Update the status of an existing room.
/// </remarks>
public class UpdateStatus(IMediator _mediator)
    : Endpoint<UpdateRoomStatusRequest, object>
{
    public override void Configure()
    {
        Put(UpdateRoomStatusRequest.Route);
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        UpdateRoomStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRoomStatusCommand(
            request.RoomId,
            (RoomStatus)request.Status);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendAsync(
                new FailureResponse(404, $"Room with ID {request.RoomId} not found"),
                statusCode: 404,
                cancellation: cancellationToken);
            return;
        }

        if (result.Status == ResultStatus.Error)
        {
            await SendAsync(
                new FailureResponse(400, result.Errors.First()),
                statusCode: 400,
                cancellation: cancellationToken);
            return;
        }

        if (result.IsSuccess)
        {
            var roomRecord = new RoomRecord(
                result.Value.Id,
                result.Value.BranchId,
                result.Value.BranchName,
                result.Value.RoomTypeId,
                result.Value.RoomTypeName,
                result.Value.BasePrice,
                result.Value.Capacity,
                result.Value.RoomNumber,
                result.Value.Status,
                result.Value.Floor);

            await SendAsync(
                new { status = 200, message = "Room status updated successfully", data = roomRecord },
                statusCode: 200,
                cancellation: cancellationToken);
            return;
        }

        await SendAsync(
            new FailureResponse(500, "An unexpected error occurred."),
            statusCode: 500,
            cancellation: cancellationToken);
    }
} 