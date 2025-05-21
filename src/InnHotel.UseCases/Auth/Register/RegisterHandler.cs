using InnHotel.Core.AuthAggregate;
using InnHotel.Core.EmployeeAggregate;
using Microsoft.AspNetCore.Identity;

namespace InnHotel.UseCases.Auth.Register;

public class RegisterHandler(
    UserManager<ApplicationUser> userManager,
    IRepository<Employee> employeeRepo
  ) : ICommandHandler<RegisterCommand, Result<UserCreatedResponse>>
{
  public async Task<Result<UserCreatedResponse>> Handle(
    RegisterCommand request,
    CancellationToken cancellationToken
  )
  {
    // 1. Create Identity user
    var user = new ApplicationUser
    {
      UserName = request.Email,
      Email = request.Email,
      EmailConfirmed = true // Users are pre-verified
    };

    var createResult = await userManager.CreateAsync(user, request.Password);
    if (!createResult.Succeeded)
      return Result<UserCreatedResponse>.Invalid(
          createResult.Errors
                      .Select(e => new ValidationError(e.Description))
                      .ToList()
      );

    // Create employee record if needed
    if (ShouldCreateEmployee(request))
    {
      var employee = new Employee(
          request.BranchId!.Value,
          request.FirstName!,
          request.LastName!,
          request.HireDate!.Value,
          request.Position!,
          user.Id
      );

      await employeeRepo.AddAsync(employee, cancellationToken);
    }
    
    return Result<UserCreatedResponse>.Success(
            new UserCreatedResponse(
                id: user.Id,
                email: user.Email,
                name: $"{request.FirstName} {request.LastName}"
            )
        );
  }

  private static bool ShouldCreateEmployee(RegisterCommand request)
    {
        return request.BranchId.HasValue
            && !string.IsNullOrWhiteSpace(request.FirstName)
            && !string.IsNullOrWhiteSpace(request.LastName)
            && request.HireDate is not null
            && !string.IsNullOrWhiteSpace(request.Position);
    }
}
