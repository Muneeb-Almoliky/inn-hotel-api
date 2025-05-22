namespace InnHotel.UseCases.Guests.List;

public record ListGuestsQuery(int PageNumber, int PageSize) : IQuery<Result<(List<GuestDTO> Items, int TotalCount)>>;