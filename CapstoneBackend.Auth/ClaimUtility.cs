using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using CapstoneBackend.Auth.Models;
using CapstoneBackend.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

[assembly:InternalsVisibleTo("CapstoneBackend.Test")]

namespace CapstoneBackend.Auth;

internal class ClaimUtility
{
    private static byte[] _key = null!;
    private static IConfiguration _configuration = null!;

    internal ClaimUtility(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    internal static string CreateToken(DatabaseUser user)
    {
        //TODO should almost certainly be different from the key used to encrypt passwords
        var keyString = _configuration.GetValue<string>(EnvironmentVariables.TOKEN_KEY)!;
        _key = Encoding.ASCII.GetBytes(keyString);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(CreateClaims(user)),
            Expires = DateTime.Now.AddDays(1), // set how long until the user will need to login again
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static IEnumerable<Claim> CreateClaims(DatabaseUser user)
    {
        //making it a list in case I need to add roles or permissions later
        var result = new List<Claim> {
            new (ClaimTypes.Actor, user.Id.ToString()),
            new (ClaimTypes.Name, user.Username),
            new (ClaimTypes.Email, user.EmailAddress)
        };
            
        return result.ToArray();
    }
}