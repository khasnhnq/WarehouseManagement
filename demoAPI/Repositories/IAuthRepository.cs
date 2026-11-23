
using demoAPI.Models;

namespace demoAPI.Repositories
{
    public interface IAuthRepository
    {
        Task<Users?> GetByUsernameAsync(string username);
    }
}
