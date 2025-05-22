namespace InnHotel.UseCases.Guests.Update;

public record UpdateGuestCommand(
    int GuestId,
    string FirstName,
    string LastName,
    string IdProofType,
    string IdProofNumber,
    string? Email,
    string? Phone,
    string? Address) : ICommand<Result<GuestDTO>>;