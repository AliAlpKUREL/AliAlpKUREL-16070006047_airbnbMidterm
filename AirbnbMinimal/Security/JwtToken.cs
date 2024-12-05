using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AirbnbMinimal.Models;
using AirbnbMinimal.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AirbnbMinimal.Security;

public class JwtToken
{
    private readonly JwtTokenOptions _jwtTokenOptions;
    
    public JwtToken(IOptions<JwtTokenOptions> jwtTokenOptions)
    {
        _jwtTokenOptions = jwtTokenOptions.Value;
    }

    public string GenerateToken(User user)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.DateOfBirth, user.BirthDate.ToString("yyyy.MM.dd")),
            new(ClaimTypes.Surname, user.Surname),
            new(ClaimTypes.Role, user.Role.Name)
        ];
        
        var securityKeyBytes = Encoding.UTF8.GetBytes(_jwtTokenOptions.SecurityKey);
        var key = new SymmetricSecurityKey(securityKeyBytes);
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            issuer: _jwtTokenOptions.Issuer,
            audience: _jwtTokenOptions.Audience,
            expires: DateTime.Now.AddMinutes(_jwtTokenOptions.ExpirationMinute),
            signingCredentials: cred
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtTokenOptions.SecurityKey); 

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtTokenOptions.Issuer, 
                ValidateAudience = true,
                ValidAudience = _jwtTokenOptions.Audience,
                ValidateLifetime = true
            }, out _);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public T? GetClaimValue<T>(ClaimsPrincipal user, string claimType)
    {
        var value = user.FindFirst(claimType)?.Value;
        if (value == null) return default;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }
    
    public int GetUserId(ClaimsPrincipal user)
    {
        return GetClaimValue<int>(user, ClaimTypes.NameIdentifier);
    }

    public string? GetUserRole(ClaimsPrincipal user)
    {
        return GetClaimValue<string>(user, ClaimTypes.Role);
    }
    
}