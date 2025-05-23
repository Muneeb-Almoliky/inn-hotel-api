using FastEndpoints;
using InnHotel.UseCases.Rooms.Get;
using InnHotel.Web.Common;

namespace InnHotel.Web.Rooms;

/// <summary>
/// Get a Room by integer ID.
/// </summary>
/// <remarks>
/// Takes a positive integer ID and returns a matching Room record.
/// </remarks>
public class GetById(IMediator _mediator)
    : Endpoint<GetRoomByIdRequest, object>
{
    public override void Configure()
    {
        Get(GetRoomByIdRequest.Route);
        Description(d => d
            .Produces<RoomRecord>(200, "application/json")
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task HandleAsync(GetRoomByIdRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetRoomQuery(request.RoomId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {
            var error = new FailureResponse(404, $"Room with ID {request.RoomId} not found");
            await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
            return;
        }

        if (result.IsSuccess)
        {
            var room = result.Value;
            Response = new RoomRecord(
                room.Id,
                room.BranchId,
                room.BranchName,
                room.RoomTypeId,
                room.RoomTypeName,
                room.BasePrice,
                room.Capacity,
                room.RoomNumber,
                room.Status,
                room.Floor);
            await SendOkAsync(Response, cancellationToken);
            return;
        }

        await SendAsync(
            new FailureResponse(500, "An unexpected error occurred."), 
            statusCode: 500, 
            cancellation: cancellationToken);
    }
}
