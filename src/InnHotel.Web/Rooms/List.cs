using FastEndpoints;
using InnHotel.UseCases.Rooms.List;
using InnHotel.Web.Common;
using Microsoft.AspNetCore.Authorization;

namespace InnHotel.Web.Rooms;

/// <summary>
/// List all Rooms with pagination support.
/// </summary>
/// <remarks>
/// Returns a paginated list of Room records.
/// </remarks>
public class List(IMediator _mediator)
    : Endpoint<PaginationRequest, object>
{
    public override void Configure()
    {
        Get(ListRoomRequest.Route);
        Description(d => d
            .Produces<PagedResponse<RoomRecord>>(200, "application/json")
            .ProducesProblem(500));
    }

    public override async Task HandleAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        var query = new ListRoomQuery(request.PageNumber, request.PageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            var (items, totalCount) = result.Value;
            var roomRecords = items.Select(room => 
                new RoomRecord(
                    room.Id,
                    room.BranchId,
                    room.BranchName,
                    room.RoomTypeId,
                    room.RoomTypeName,
                    room.BasePrice,
                    room.Capacity,
                    room.RoomNumber,
                    room.Status,
                    room.Floor)).ToList();

            var response = new PagedResponse<RoomRecord>(
                roomRecords, 
                totalCount, 
                request.PageNumber, 
                request.PageSize);

            await SendOkAsync(response, cancellationToken);
            return;
        }

        await SendAsync(
            new FailureResponse(500, "An unexpected error occurred."), 
            statusCode: 500, 
            cancellation: cancellationToken);
    }
}
