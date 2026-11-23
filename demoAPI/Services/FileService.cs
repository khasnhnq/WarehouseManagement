using demoAPI.DTOs;
using demoAPI.Models;
using demoAPI.Repositories;

namespace demoAPI.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        // Upload file
        public async Task<FileResponseDto> UploadAsync(
            UploadFileDto dto)
        {
            var file = dto.File;

            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ.");

            // Giới hạn file 10 MB
            if (file.Length > 10 * 1024 * 1024)
                throw new ArgumentException("File không được vượt quá 10 MB.");

            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            var storedFile = new StoredFile
            {
                FileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType
                    ?? "application/octet-stream",
                Content = memoryStream.ToArray(),
                Length = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            var result = await _fileRepository.AddAsync(storedFile);

            return new FileResponseDto
            {
                Id = result.Id,
                FileName = result.FileName,
                Size = result.Length,
                UploadedAt = result.UploadedAt
            };
        }

        // Lấy danh sách file
        public async Task<List<FileResponseDto>> GetAllAsync()
        {
            var files = await _fileRepository.GetAllAsync();

            return files.Select(f => new FileResponseDto
            {
                Id = f.Id,
                FileName = f.FileName,
                Size = f.Length,
                UploadedAt = f.UploadedAt
            }).ToList();
        }

        // Tải file
        public async Task<FileDownloadDto?> DownloadAsync(int id)
        {
            var file = await _fileRepository.GetByIdAsync(id);

            if (file == null)
                return null;

            return new FileDownloadDto
            {
                Content = file.Content,
                ContentType = file.ContentType,
                FileName = file.FileName
            };
        }

        // Xóa file
        public async Task<bool> DeleteAsync(int id)
        {
            return await _fileRepository.DeleteAsync(id);
        }

        public async Task<FileResponseDto?> GetByIdAsync(int id)
        {
            var file = await _fileRepository.GetByIdAsync(id);

            if (file == null)
            {
                return null;
            }

            return new FileResponseDto
            {
                Id = file.Id,
                FileName = file.FileName,
                Size = file.Length,
                UploadedAt = file.UploadedAt
            };
        }
    }
}