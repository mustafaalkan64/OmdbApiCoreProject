using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OmdbApi.DAL.Consts;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Helpers;
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
        private string secretKey;

        public UserService(IUnitOfWork unit)
        {
            _uow = unit;
            secretKey = AppSettingsParameters.Secret;
        }

        public async Task<string> Authenticate(string username, string password)
        {
            var user = await _uow.UserRepository.FindBy(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return string.Empty;

            var token = JWTHelper.CreateToken(user, secretKey);
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
                var token = JWTHelper.CreateToken(user, secretKey);
                return token;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
