namespace InnHotel.UseCases.Guests.Get;

public record GetGuestQuery(int guestId) : IQuery<Result<GuestDTO>>;
