using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CareerAI.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CareerAI.Infrastructure.Authentication;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(
        Guid userId,
        string email,
        string role)
    {
        // Read JWT settings from appsettings.json
        var jwtSettings = _configuration.GetSection("Jwt");

        var key = jwtSettings["Key"]
            ?? throw new InvalidOperationException(
                "JWT Key is missing from appsettings.json.");

        var issuer = jwtSettings["Issuer"]
            ?? throw new InvalidOperationException(
                "JWT Issuer is missing from appsettings.json.");

        var audience = jwtSettings["Audience"]
            ?? throw new InvalidOperationException(
                "JWT Audience is missing from appsettings.json.");

        var expirationValue = jwtSettings["ExpirationMinutes"];

        if (!int.TryParse(expirationValue, out var expirationMinutes))
        {
            expirationMinutes = 60;
        }

        // ========================================
        // Create Claims
        // ========================================

        var claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                userId.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                email),

            new Claim(
                ClaimTypes.Role,
                role)
        };

        // ========================================
        // Create Security Key
        // ========================================

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        // ========================================
        // Create Signing Credentials
        // ========================================

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        // ========================================
        // Create JWT
        // ========================================

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        // ========================================
        // Convert JWT to String
        // ========================================

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}