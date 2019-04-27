using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OmdbApi.DAL.Consts;
using OmdbApi.DAL.EFDbContext;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OmdbApi.Business.Helpers
{
    public static class JWTManager
    {
        public static string CreateToken(User user, string secretkey)
        {
            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username?.ToString() ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() ?? ""),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email?.ToString() ?? ""),
                    new Claim(StaticVariables.UserId, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, RoleType.User),
                    new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretkey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://github.com/mustafaalkan64/OmdbApiCoreProject",
                audience: "https://github.com/mustafaalkan64/OmdbApiCoreProject",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            var usertoken = new JwtSecurityTokenHandler().WriteToken(token);
            return usertoken;
        }
    }
}
