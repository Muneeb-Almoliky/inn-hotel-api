using InnHotel.UseCases.Rooms.Update;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Rooms;

/// <summary>
/// Update an existing Room.
/// </summary>
/// <remarks>
/// Update an existing Room by providing updated details.
/// </remarks>
public class Update(IMediator _mediator)
    : Endpoint<UpdateRoomRequest, object>
{
    public override void Configure()
    {
        Put(UpdateRoomRequest.Route);
        Roles(AuthRoles.SuperAdmin, AuthRoles.Admin);
    }

    public override async Task HandleAsync(
        UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRoomCommand(
            request.RoomId,
            request.RoomTypeId,
            request.RoomNumber,
            (RoomStatus)request.Status,
            request.Floor);

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
                new { status = 200, message = "Room updated successfully", data = roomRecord },
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
