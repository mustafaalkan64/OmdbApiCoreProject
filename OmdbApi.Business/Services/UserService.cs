﻿using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using OmdbApi.DAL.Consts;
using OmdbApi.Business.Helpers;
using OmdbApi.Business.Validations;
using OmdbApi.DAL.EFDbContext;
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

        public async Task<WebApiResponse> Authenticate(UserLoginDto userLoginDto)
        {
            try
            {
                var errors = ValidatorUtility.FluentValidate(new UserLoginValidator(), userLoginDto);
                if(errors != "")
                {
                    return new WebApiResponse()
                    {
                        Response = errors,
                        Status = false
                    };
                }

                var user = await _uow.UserRepository.FindBy(x => x.Username.Equals(userLoginDto.Username) || x.Email.Equals(userLoginDto.Username));

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
                    if (UserPasswordHashManager.AreEqual(userLoginDto.Password, user.Hash, user.Salt))
                    {
                        var token = JWTManager.CreateToken(user, secretKey);
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
                _logger.LogError(ex, "Exception Error During Login User", userLoginDto.Username);
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
                user.Salt = UserPasswordHashManager.CreateSalt(10);
                user.Hash = UserPasswordHashManager.GenerateHash(userDto.Password, user.Salt);

                await _uow.UserRepository.Add(user);
                await _uow.Commit();
                
                // Get Token
                var token = JWTManager.CreateToken(user, secretKey);
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

                var user = await _uow.UserRepository.FindBy(x => x.Id.Equals(changePasswordModel.UserId));
                if (user == null) 
                    return null;
                else
                {
                    if (UserPasswordHashManager.AreEqual(changePasswordModel.CurrentPassword, user.Hash, user.Salt))
                    {
                        user.Salt = UserPasswordHashManager.CreateSalt(10);
                        user.Hash = UserPasswordHashManager.GenerateHash(changePasswordModel.NewPassword, user.Salt);
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
           return await _uow.UserRepository.Any(x => x.Username.Equals(username));
        }


        /// <summary>
        /// Check If Email Exists In Users Table
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private async Task<bool> CheckEmail(string email)
        {
            return await _uow.UserRepository.Any(x => x.Email.Equals(email));
        }

    }
}
