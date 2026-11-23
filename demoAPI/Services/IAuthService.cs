
using demoAPI.DTOs;

namespace demoAPI.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto dto);
    }
}
