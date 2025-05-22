using InnHotel.Core.GuestAggregate;

namespace InnHotel.UseCases.Guests.List;

public class ListGuestsHandler(IReadRepository<Guest> _repository)
	: IQueryHandler<ListGuestsQuery, Result<List<GuestDTO>>>
{
	public async Task<Result<List<GuestDTO>>> Handle(ListGuestsQuery request, CancellationToken cancellationToken)
	{
		var guests = await _repository.ListAsync(cancellationToken);

		return guests.Select(entity => new GuestDTO(
						entity.Id,
						entity.FirstName,
						entity.LastName,
						entity.IdProofType,
						entity.IdProofNumber,
						entity.Email,
						entity.Phone,
						entity.Address ?? "")).ToList();
	}
}