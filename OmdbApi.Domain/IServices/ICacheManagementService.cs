using System.Threading.Tasks;

namespace OmdbApi.Domain.IServices
{
    public interface ICacheManagementService
    {
        Task<bool> Clear();
    }
}
