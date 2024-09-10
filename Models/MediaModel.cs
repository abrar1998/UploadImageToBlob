namespace UploadImageToBlob.Models
{
    public class MediaModel
    {
        public IFormFile? photo { get; set; }
        public IFormFile? pdf { get; set; }
        public IFormFile? video { get; set; }
        public string? PhotoUrl { get; set; }
        public string? PdfUrl { get; set; }
        public string? VideoUrl { get; set; }
    }
}
