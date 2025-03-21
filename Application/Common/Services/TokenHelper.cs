using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Application.Common.Services;

public class TokenHelper : ITokenHelper
{
    private readonly IConfiguration _configuration;

    public TokenHelper(IConfiguration configuration)
    {
        _configuration = configuration; 
    }

    public string GenerateToken(Guid userId)
    {
        var options = _configuration.GetSection("JwtOptions");

        var claims = new List<Claim>
        {
            new Claim("userId", userId.ToString())
        };

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: options["Issuer"],
            audience: options["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(
                GetSymmetricSecurityKey(userId), 
                SecurityAlgorithms.Sha512)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

    public bool ValidateToken(string token, out Guid userId)
    {
        var handler = new JwtSecurityTokenHandler();

        var jwt = handler.ReadJwtToken(token);
        userId = Guid.Parse(jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        IPrincipal principal = handler.ValidateToken(token, GetTokenValidationParameters(userId), out var validatedToken);

        return true;
    }

    private SymmetricSecurityKey GetSymmetricSecurityKey(Guid userId)
    {
        var options = _configuration.GetSection("JwtOptions");

        var key = new byte[options["Key"].Length + 16];
        Array.Copy(Encoding.UTF8.GetBytes(options["Key"]), key, options["Key"].Length);
        Array.Copy(userId.ToByteArray(), 0, key, options["Key"].Length, 16);

        return new SymmetricSecurityKey(key);
    }

    private TokenValidationParameters GetTokenValidationParameters(Guid userId)
    {
        var options = _configuration.GetSection("JwtOptions");

        return new TokenValidationParameters()
        {
            ValidateLifetime = Convert.ToBoolean(options["ValidateLifetime"]),
            ValidateAudience = Convert.ToBoolean(options["ValidateAudience"]),
            ValidateIssuer = Convert.ToBoolean(options["ValidateIssuer"]),
            ValidIssuer = options["ValidIssuer"],
            ValidAudience = options["ValidAudience"],
            IssuerSigningKey = GetSymmetricSecurityKey(userId)
        };
    }
}
