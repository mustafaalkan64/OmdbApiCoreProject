using AutoMapper;
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
        private readonly IMapper _mapper;
        private string secretKey;

        public UserService(IUnitOfWork unit, IMapper mapper)
        {
            _uow = unit;
            secretKey = AppSettingsParameters.Secret;
            _mapper = mapper;
        }

        public async Task<WebApiResponse> Authenticate(string username, string password)
        {
            var user = await _uow.UserRepository.FindBy(x => x.Username == username || x.Email == username);

            // return null if user not found
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
                        Response = "Password is wrong",
                        Status = false
                    };
                }
            }
        }

        public async Task<WebApiResponse> Register(UserDto userDto)
        {
            try
            {
                UserValidator validator = new UserValidator();
                ValidationResult result = validator.Validate(userDto);
                if (!result.IsValid)
                {
                    string allMessages = result.ToString("~");     // In this case, each message will be separated with a `~` 

                    return new WebApiResponse()
                    {
                        Response = allMessages,
                        Status = false
                    };
                }

                if(!(await CheckUserName(userDto.Username)))
                {
                    return new WebApiResponse()
                    {
                        Response = "This UserName Allready Exists. Please Enter Different UserName",
                        Status = false
                    };
                }

                if (!(await CheckEmail(userDto.Email)))
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
                throw ex;
            }

        }

        public async Task<WebApiResponse> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            try
            {
                ChangePasswordValidator validator = new ChangePasswordValidator();
                ValidationResult result = validator.Validate(changePasswordModel);
                if (!result.IsValid)
                {
                    string allMessages = result.ToString("~");     // In this case, each message will be separated with a `~` 

                    return new WebApiResponse()
                    {
                        Response = allMessages,
                        Status = false
                    };
                }

                var user = await _uow.UserRepository.FindBy(x => x.Id == changePasswordModel.UserId);
                if (user == null)
                {
                    return new WebApiResponse()
                    {
                        Response = "User Not Found",
                        Status = false
                    };
                }
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
