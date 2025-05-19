namespace InnHotel.Core.GuestAggregate;

public class Guest(
    string firstName,
    string lastName,
    string idProofType,
    string idProofNumber,
    string? email = null,
    string? phone = null,
    string? address = null
) : EntityBase, IAggregateRoot
{
    public string FirstName { get; } = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
    public string LastName { get; } = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
    public string IdProofType { get; } = Guard.Against.NullOrEmpty(idProofType, nameof(idProofType));
    public string IdProofNumber { get; } = Guard.Against.NullOrEmpty(idProofNumber, nameof(idProofNumber));
    public string? Email { get; private set; } = email;
    public string? Phone { get; private set; } = phone;
    public string? Address { get; private set; } = address;
}

