using demoAPI.Models;

namespace demoAPI.Repositories
{
    public interface IFileRepository
    {
        Task<StoredFile> AddAsync(StoredFile file);
        Task<List<StoredFile>> GetAllAsync();
        Task<StoredFile?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}