using OmdbApi.DAL.EFDbContext;
using OmdbApi.DAL.Models;
using System.Threading.Tasks;

namespace OmdbApi.Domain.IServices
{
    public interface IUserService
    {
        Task<WebApiResponse> Authenticate(UserLoginDto userLoginDto);
        Task<WebApiResponse> Register(UserDto user);
        Task<WebApiResponse> ChangePassword(ChangePasswordModel changePasswordModel);
    }
}
