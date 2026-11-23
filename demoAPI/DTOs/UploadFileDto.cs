using Microsoft.AspNetCore.Http;

namespace demoAPI.DTOs
{
    public class UploadFileDto
    {
        public required IFormFile File { get; set; }
    }
}