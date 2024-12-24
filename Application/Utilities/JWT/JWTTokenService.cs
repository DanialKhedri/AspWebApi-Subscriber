#region Usings
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
#endregion

namespace Application.Utilities.JWT;

public static class JWTTokenService
{


    // Generate Access Token
    public static async Task<string> GenerateToken(User user, IConfiguration _configuration)
    {

        var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:Key"]));
        var credential = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

        var Claims = new List<Claim>()
        {

            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name,user.UserName),

        };

        var Token = new JwtSecurityToken(
            issuer: _configuration["JWTSettings:Issuer"],
            audience: _configuration["JWTSettings:Audience"],
            claims: Claims,
            expires: DateTime.Now.AddHours(Convert.ToInt32(_configuration["JWTSettings:Duration"])),
            signingCredentials: credential
            );


        return new JwtSecurityTokenHandler().WriteToken(Token);

    }


    //  Generate Refresh Token
    public static string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString(); // تولید یک رشته یکتا
    }


}
