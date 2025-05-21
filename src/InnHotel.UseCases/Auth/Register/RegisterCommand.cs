using InnHotel.Core.AuthAggregate;

namespace InnHotel.UseCases.Auth.Register;

/// <summary>
/// Command to register a new user account. 
/// Includes optional employee information if applicable.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
/// <param name="BranchId">Optional: Branch ID if the user is an employee.</param>
/// <param name="FirstName">Optional: First name of the employee.</param>
/// <param name="LastName">Optional: Last name of the employee.</param>
/// <param name="HireDate">Optional: Hire date of the employee.</param>
/// <param name="Position">Optional: Job position of the employee.</param>

public record RegisterCommand(
    string Email,
    string Password,
    int? BranchId,             
    string? FirstName,
    string? LastName,
    DateOnly? HireDate,
    string? Position) : ICommand<Result<UserCreatedResponse>>;
