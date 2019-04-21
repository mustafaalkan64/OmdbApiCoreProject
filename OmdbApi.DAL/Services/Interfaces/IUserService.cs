using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Models;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Services.Interfaces
{
    public interface IUserService
    {
        Task<WebApiResponse> Authenticate(string username, string password);
        Task<WebApiResponse> Register(UserDto user);
        Task<WebApiResponse> ChangePassword(ChangePasswordModel changePasswordModel);
    }
}
