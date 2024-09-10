using System.ComponentModel.DataAnnotations.Schema;

namespace UploadImageToBlob.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        [NotMapped]
        public IFormFile photo { get; set; }
        public string? photoUrl { get; set; }
    }
}
