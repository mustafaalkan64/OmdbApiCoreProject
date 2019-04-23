using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using OmdbApi.Business.Validations;
using OmdbApi.DAL.Consts;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Helpers;
using OmdbApi.DAL.Models;
using OmdbApi.DAL.Uow;
using OmdbApi.Domain.IServices;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OmdbApi.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private string secretKey;
        private readonly ILogger<MovieService> _logger;


        public UserService(IUnitOfWork unit, IMapper mapper, ILogger<MovieService> logger)
        {
            _uow = unit;
            secretKey = AppSettingsParameters.Secret;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WebApiResponse> Authenticate(string username, string password)
        {
            try
            {
                var user = await _uow.UserRepository.FindBy(x => x.Username == username || x.Email == username);

                if (user == null)
                {
                    return new WebApiResponse()
                    {
                        Response = "Username or Email Not Found",
                        Status = false
                    };
                }
                else
                {
                    if (UserPasswordHashHelper.AreEqual(password, user.Hash, user.Salt))
                    {
                        var token = JWTHelper.CreateToken(user, secretKey);
                        return new WebApiResponse()
                        {
                            Response = token,
                            Status = true
                        };
                    }
                    else
                    {
                        return new WebApiResponse()
                        {
                            Response = "Password is Wrong",
                            Status = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Error During Login User", username, password);
                throw ex;
            }
            
        }

        public async Task<WebApiResponse> Register(UserDto userDto)
        {
            try
            {
                ValidatorUtility.FluentValidate(new UserValidator(), userDto);
                if(await CheckUserName(userDto.Username))
                {
                    return new WebApiResponse()
                    {
                        Response = "This UserName Allready Exists. Please Enter Different UserName",
                        Status = false
                    };
                }

                if (await CheckEmail(userDto.Email))
                {
                    return new WebApiResponse()
                    {
                        Response = "This Email Address Allready Exists. Please Enter Different Email Address",
                        Status = false
                    };
                }

                // return null if user not found
                if (userDto == null)
                    return new WebApiResponse()
                    {
                        Response = "User can not be null",
                        Status = false
                    };

                // Register
                var user = _mapper.Map<User>(userDto);
                user.Salt = UserPasswordHashHelper.CreateSalt(10);
                user.Hash = UserPasswordHashHelper.GenerateHash(userDto.Password, user.Salt);

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
                _logger.LogError(ex, "Exception Error During User Register", userDto);
                throw ex;
            }

        }

        public async Task<WebApiResponse> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            try
            {
                ValidatorUtility.FluentValidate(new ChangePasswordValidator(), changePasswordModel);

                var user = await _uow.UserRepository.FindBy(x => x.Id == changePasswordModel.UserId);
                if (user == null) 
                    return null;
                else
                {
                    if (UserPasswordHashHelper.AreEqual(changePasswordModel.CurrentPassword, user.Hash, user.Salt))
                    {
                        user.Salt = UserPasswordHashHelper.CreateSalt(10);
                        user.Hash = UserPasswordHashHelper.GenerateHash(changePasswordModel.NewPassword, user.Salt);
                        await _uow.UserRepository.Update(user);
                        await _uow.Commit();

                        return new WebApiResponse()
                        {
                            Response = "Password Changed Successfuly",
                            Status = true
                        };
                    }
                    else
                    {
                        return new WebApiResponse()
                        {
                            Response = "Current Password is Wrong",
                            Status = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Error During Change Password", changePasswordModel);
                throw ex;
            }

        }

        /// <summary>
        /// Check If User Name Exists In Users Table
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private async Task<bool> CheckUserName(string username)
        {
           return await _uow.UserRepository.Any(x => x.Username == username);
        }


        /// <summary>
        /// Check If Email Exists In Users Table
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private async Task<bool> CheckEmail(string email)
        {
            return await _uow.UserRepository.Any(x => x.Email == email);
        }

    }
}
