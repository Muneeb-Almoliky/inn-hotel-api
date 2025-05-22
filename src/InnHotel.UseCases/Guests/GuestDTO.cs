namespace InnHotel.UseCases.Guests;
public record GuestDTO(int Id, string FirstName, string LastName, string IdProofType, string IdProofNumber, string? Email, string? Phone, string? Address);
