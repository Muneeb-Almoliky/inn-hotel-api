using System.Security.Claims;
using InnHotel.Core.AuthAggregate;
using InnHotel.Core.AuthAggregate.Specifications;
using InnHotel.Core.EmployeeAggregate;
using InnHotel.Core.Interfaces;
using InnHotel.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace InnHotel.UseCases.Auth.Refresh;

public class RefreshTokenHandler(
  UserManager<ApplicationUser> _userManager,
  IRepository<RefreshToken> _refreshTokenRepo,
  ITokenService _tokenService,
  IConfiguration config)
  : ICommandHandler<RefreshTokenCommand, Result<AuthResult>>
{
  private readonly IConfiguration _config = config;
  public async Task<Result<AuthResult>> Handle(
      RefreshTokenCommand request,
      CancellationToken ct)
  {
    var spec = new RefreshTokenByTokenSpec(request.Token);

    var refreshToken = await _refreshTokenRepo.FirstOrDefaultAsync(spec);

    if (refreshToken == null)
      return Result<AuthResult>.Unauthorized("Invalid refresh token");

    if (refreshToken.Revoked || refreshToken.ExpiresAt < DateTime.UtcNow)
      return Result<AuthResult>.Unauthorized("Expired or revoked token");

    var user = await _userManager.FindByIdAsync(refreshToken.UserId);
    if (user == null)
      return Result<AuthResult>.NotFound("User not found");



    var roles = await _userManager.GetRolesAsync(user);
    var claims = GenerateClaims(user, roles);

    var newAccessToken = _tokenService.GenerateAccessToken(claims);
    var newRefreshToken = _tokenService.GenerateRefreshToken();

    // Update refresh token in database (token rotation)
    await RotateTokens(refreshToken, newRefreshToken, request.IpAddress);

    return Result<AuthResult>.Success(
        new AuthResult(
            accessToken: newAccessToken,
            refreshToken: newRefreshToken,
            email: user.Email,
            roles: roles.ToList()
        )
    );
  }

  private List<Claim> GenerateClaims(ApplicationUser user, IList<string> roles)
  {
    var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
    return claims;
  }

  private async Task RotateTokens(
        RefreshToken oldToken,
        string newTokenValue,
        string? ipAddress)
  {
    // Revoke old token
    oldToken.Revoked = true;
    oldToken.RevokedAt = DateTime.UtcNow;
    await _refreshTokenRepo.UpdateAsync(oldToken);

    // Add new token
    var newToken = new RefreshToken(
        token: newTokenValue,
        userId: oldToken.UserId,
        expiresAt: DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenExpiryDays"]!)),
        deviceInfo: oldToken.DeviceInfo,
        ipAddress: ipAddress
    );

    await _refreshTokenRepo.AddAsync(newToken);
    await _refreshTokenRepo.SaveChangesAsync();
  }
}

