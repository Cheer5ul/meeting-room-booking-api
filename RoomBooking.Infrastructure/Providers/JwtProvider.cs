using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RoomBooking.Application.Interfaces.Auth.Providers;
using RoomBooking.Core.Models.User;

namespace RoomBooking.Infrastructure.Providers;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(User user)
    {
        Claim[] claims = [new Claim("userId", user.Id.ToString())];
        
        //algorithm for token hashing
        SigningCredentials signingCredentials = new SigningCredentials(
            key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), //security key for coding and decoding the token
            algorithm: SecurityAlgorithms.HmacSha256);
        
        //creating a token
        JwtSecurityToken token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(_options.ExpiresHours));
        
        //creating a string from JwtSecurityToken object,
        //cuz JwtSecurityToken an object in memory which cannot be sent in network as is
        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token); 
        
        return tokenValue;
    }
}