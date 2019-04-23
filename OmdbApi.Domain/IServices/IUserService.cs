using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Models;
using System.Threading.Tasks;

namespace OmdbApi.Domain.IServices
{
    public interface IUserService
    {
        Task<WebApiResponse> Authenticate(string username, string password);
        Task<WebApiResponse> Register(UserDto user);
        Task<WebApiResponse> ChangePassword(ChangePasswordModel changePasswordModel);
    }
}
