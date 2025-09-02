using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NanoHealthSuite.Data.Models;

namespace NanoHealthSuite.TrackingSystem.Processors;

public class TokenServiceProvider : ITokenServiceProvider
{
    private readonly IConfiguration _configuration;

    public TokenServiceProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AuthResponseDto GenerateAccessToken(User user)
    {
        var expirationTime = DateTime.Now.AddMinutes(int.Parse(_configuration["JWT:LifeTimeInMinutes"]));
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
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

    private List<Claim> GenerateUserClaim(User user)
    {
        return
        [
            new Claim(ClaimTypes.Name, user.Name),
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