using demoAPI.Data;
using demoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace demoAPI.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly AppDbContext _context;

        public FileRepository(AppDbContext context)
        {
            _context = context;
        }

        // Thêm file
        public async Task<StoredFile> AddAsync(StoredFile file)
        {
            _context.StoredFiles.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }

        // Lấy danh sách file
        public async Task<List<StoredFile>> GetAllAsync()
        {
            return await _context.StoredFiles
                .AsNoTracking()
                .ToListAsync();
        }

        // Lấy file theo Id
        public async Task<StoredFile?> GetByIdAsync(int id)
        {
            return await _context.StoredFiles
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        // Xóa file
        public async Task<bool> DeleteAsync(int id)
        {
            var file = await GetByIdAsync(id);

            if (file == null)
                return false;

            _context.StoredFiles.Remove(file);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}