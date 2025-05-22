using InnHotel.Core.AuthAggregate;

namespace InnHotel.UseCases.Auth.Login;
public record LoginCommand(
    string Email,
    string Password
) : ICommand<Result<AuthResult>>;
