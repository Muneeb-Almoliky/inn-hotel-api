using System.Security.Claims;
using Ardalis.Result;
using InnHotel.Core.AuthAggregate;
using InnHotel.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace InnHotel.UseCases.Auth.Login;

public class LoginHandler(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IRepository<RefreshToken> refreshTokenRepo,
    IConfiguration config)
    : ICommandHandler<LoginCommand, Result<AuthResponse>>
{
  private readonly IConfiguration _config = config;

  public async Task<Result<AuthResponse>> Handle(
      LoginCommand request,
      CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
          return Result<AuthResponse>.Unauthorized("Invalid Credintials");

        if (!await userManager.CheckPasswordAsync(user, request.Password))
            return Result<AuthResponse>.Unauthorized("Invalid Credintials");

        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Name, user.UserName ?? string.Empty)
            };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var accessToken = tokenService.GenerateAccessToken(claims);
        var refreshToken = tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken(
          token: refreshToken,
          userId: user.Id,
          expiresAt: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:RefreshTokenExpiryDays"]!)),
          deviceInfo: null,
          ipAddress: null
        );

        await refreshTokenRepo.AddAsync(refreshTokenEntity, cancellationToken);

        return Result<AuthResponse>.Success(
          new AuthResponse(
            accessToken: accessToken,
            refreshToken: refreshToken,
            email: user.Email,
            roles: roles.ToList()
          )
        );
    }
}
