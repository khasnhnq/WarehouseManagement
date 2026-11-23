using demoAPI.DTOs;

namespace demoAPI.Services
{
    public interface IFileService
    {
        Task<FileResponseDto> UploadAsync(UploadFileDto dto);
        Task<List<FileResponseDto>> GetAllAsync();
        Task<FileDownloadDto?> DownloadAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<FileResponseDto?> GetByIdAsync(int id);
    }
}