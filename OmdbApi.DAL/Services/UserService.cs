using FluentValidation.Results;
using OmdbApi.DAL.Consts;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Helpers;
using OmdbApi.DAL.Models;
using OmdbApi.DAL.Services.Interfaces;
using OmdbApi.DAL.Uow;
using OmdbApi.DAL.Validations;
using System;
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

        public async Task<WebApiResponse> Register(User user)
        {
            try
            {
                UserValidator validator = new UserValidator();
                ValidationResult result = validator.Validate(user);
                if (!result.IsValid)
                {
                    string allMessages = result.ToString("~");     // In this case, each message will be separated with a `~` 

                    return new WebApiResponse()
                    {
                        Response = allMessages,
                        Status = false
                    };
                }

                if(!(await CheckUserName(user.Username)))
                {
                    return new WebApiResponse()
                    {
                        Response = "This UserName Allready Exists. Please Enter Different UserName",
                        Status = false
                    };
                }

                if (!(await CheckEmail(user.Email)))
                {
                    return new WebApiResponse()
                    {
                        Response = "This Email Address Allready Exists. Please Enter Different Email Address",
                        Status = false
                    };
                }

                // return null if user not found
                if (user == null)
                    return new WebApiResponse()
                    {
                        Response = "User can not be null",
                        Status = false
                    };

                // Register
                await _uow.UserRepository.Add(user);
                await _uow.Commit();
                
                // Get Token
                var token = JWTHelper.CreateToken(user, secretKey);
                return new WebApiResponse()
                {
                    Response = token,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<bool> CheckUserName(string username)
        {
            var user = await _uow.UserRepository.FindBy(x => x.Username == username);

            if (user == null)
                return true;

            return false;
        }


        private async Task<bool> CheckEmail(string email)
        {
            var user = await _uow.UserRepository.FindBy(x => x.Email == email);

            if (user == null)
                return true;

            return false;
        }

    }
}
