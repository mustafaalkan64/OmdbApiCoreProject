using OmdbApi.DAL.Entities;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Services.Interfaces
{
    public interface ICacheManagementService
    {
        Task<bool> Clear();
    }
}
