using demoAPI.DTOs;
using demoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace demoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // POST: api/File/upload
        // Upload file
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(
            [FromForm] UploadFileDto dto)
        {
            try
            {
                var result = await _fileService.UploadAsync(dto);

                return Ok(new
                {
                    message = "Upload file thành công.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/File
        // Lấy danh sách file
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var files = await _fileService.GetAllAsync();

            return Ok(files);
        }

        // GET: api/File/download/1
        // Download file
        [HttpGet("download/{id:int}")]
        public async Task<IActionResult> Download(int id)
        {
            var file = await _fileService.DownloadAsync(id);

            if (file == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy file."
                });
            }

            return File(
                file.Content,
                file.ContentType,
                file.FileName
            );
        }

        // DELETE: api/File/1
        // Xóa file
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _fileService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy file cần xóa."
                });
            }

            return Ok(new
            {
                message = "Xóa file thành công."
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var file = await _fileService.GetByIdAsync(id);

            if (file == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy file."
                });
            }

            return Ok(file);
        }
    }
}