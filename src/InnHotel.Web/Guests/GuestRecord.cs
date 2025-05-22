namespace InnHotel.Web.Guests;

public record GuestRecord(int Id, string FirstName, string LastName, string IdProofType, string IdProofNumber, string? Email, string? Phone, string? Address);
