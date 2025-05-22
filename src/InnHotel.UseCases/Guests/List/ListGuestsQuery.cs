namespace InnHotel.UseCases.Guests.List;

public record ListGuestsQuery() : IQuery<Result<List<GuestDTO>>>;