namespace demoAPI.DTOs
{
    public class FileResponseDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}