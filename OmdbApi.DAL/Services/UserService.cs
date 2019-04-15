using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Services.Interfaces;
using OmdbApi.DAL.Uow;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork unit, IConfiguration configuration)
        {
            _uow = unit;
            _configuration = configuration;
        }

        public async Task<string> Authenticate(string username, string password)
        {
            var user = await _uow.UserRepository.FindBy(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return string.Empty;

            var token = CreateToken(user);
            return token;
        }

        public async Task<string> Register(User user)
        {
            try
            {
                // return null if user not found
                if (user == null)
                    return null;

                // Register
                await _uow.UserRepository.Add(user);
                await _uow.Commit();

                // Get Token
                var token = CreateToken(user);
                return token;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string CreateToken(User user)
        {
            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username?.ToString() ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() ?? ""),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email?.ToString() ?? ""),
                    new Claim("UserId", user.Id.ToString())
            };

            var secretkey = _configuration.GetValue<string>("AppSettings:Secret");
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
