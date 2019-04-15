using OmdbApi.DAL.Entities;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<string> Register(User user);
    }
}
