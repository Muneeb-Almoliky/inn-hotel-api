namespace InnHotel.UseCases.Guests.Delete;

public record DeleteGuestCommand(int GuestId) : ICommand<Result>;

