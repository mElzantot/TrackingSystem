using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NanoHealthSuite.Data.Models;
using NanoHealthSuite.TrackingSystem.Helpers;

namespace NanoHealthSuite.TrackingSystem.Processors;

public class TokenServiceProvider : ITokenServiceProvider
{
    private readonly JWTSettings _jwtSettings;

    public TokenServiceProvider(IOptions<JWTSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public AuthResponseDto GenerateAccessToken(User user)
    {
        var expirationTime = DateTime.Now.AddMinutes(_jwtSettings.LifeTimeInMinutes);
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            claims: GenerateUserClaim(user),
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            expires: expirationTime,
            notBefore: DateTime.Now
        );
        return new AuthResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = GenerateRefreshToken(),
            ExpiryDate = expirationTime
        };
    }

    public Guid GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return userIdClaim == null ? Guid.Empty : Guid.Parse(userIdClaim.Value);
    }

    private List<Claim> GenerateUserClaim(User user)
    {
        return
        [
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        ];
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}