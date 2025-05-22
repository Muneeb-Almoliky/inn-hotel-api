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
    public string FirstName { get; private set; } = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
    public string LastName { get; private set; } = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
    public string IdProofType { get; private set; } = Guard.Against.NullOrEmpty(idProofType, nameof(idProofType));
    public string IdProofNumber { get; private set; } = Guard.Against.NullOrEmpty(idProofNumber, nameof(idProofNumber));
    public string? Email { get; private set; } = email;
    public string? Phone { get; private set; } = phone;
    public string? Address { get; private set; } = address;

    public void UpdateDetails(
        string firstName,
        string lastName,
        string idProofType,
        string idProofNumber,
        string? email = null,
        string? phone = null,
        string? address = null)
    {
        FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
        IdProofType = Guard.Against.NullOrEmpty(idProofType, nameof(idProofType));
        IdProofNumber = Guard.Against.NullOrEmpty(idProofNumber, nameof(idProofNumber));
        Email = email;
        Phone = phone;
        Address = address;
    }
}

