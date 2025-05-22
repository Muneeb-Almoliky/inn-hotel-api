namespace InnHotel.UseCases.Auth.Refresh;

public record RefreshTokenCommand(
    string Token,
    string? IpAddress,
    string? DeviceInfo
) : ICommand<Result<AuthResult>>;

