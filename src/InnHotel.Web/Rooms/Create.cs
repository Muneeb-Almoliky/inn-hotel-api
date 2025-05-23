using InnHotel.UseCases.Rooms.Create;
using InnHotel.Web.Common;
using AuthRoles = InnHotel.Core.AuthAggregate.Roles;

namespace InnHotel.Web.Rooms;

/// <summary>
/// Create a new Room.
/// </summary>
/// <remarks>
/// Creates a new Room with the provided details.
/// </remarks>
public class Create(IMediator _mediator)
    : Endpoint<CreateRoomRequest, object>
{
    public override void Configure()
    {
    Post(CreateRoomRequest.Route);
    Roles(AuthRoles.SuperAdmin, AuthRoles.Admin);
    Summary(s =>
        {
            s.ExampleRequest = new CreateRoomRequest
            {
                BranchId = 1,
                RoomTypeId = 1,
                RoomNumber = "101",
                Status = RoomStatus.Available,
                Floor = 1
            };
        });
    }

    public override async Task HandleAsync(
        CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRoomCommand(
            request.BranchId,
            request.RoomTypeId,
            request.RoomNumber!,
            request.Status,
            request.Floor);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendAsync(
                new FailureResponse(404, result.Errors.First()),
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
                new { status = 201, message = "Room created successfully", data = roomRecord },
                statusCode: 201,
                cancellation: cancellationToken);
            return;
        }

        await SendAsync(
            new FailureResponse(500, "An unexpected error occurred."),
            statusCode: 500,
            cancellation: cancellationToken);
    }
}
