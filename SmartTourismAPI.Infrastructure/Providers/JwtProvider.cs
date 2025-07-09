using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartTourismAPI.Domain.Entities;
using SmartTourismAPI.Application.Abstractions.Providers;
using SmartTourismAPI.Infrastructure.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartTourismAPI.Infrastructure.Providers;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider {
    private readonly JwtOptions _options = options.Value;

    public string GenerateToken(MUser user) {
        var claims = new Claim[] {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Name, user.Username)
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddMinutes(60),
            signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenValue;
    }
}
