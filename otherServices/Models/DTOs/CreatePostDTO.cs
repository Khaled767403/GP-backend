using System.ComponentModel.DataAnnotations;

namespace WebAPIDotNet.DTOs
{
    public class CreatePostDTO
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        [StringLength(255)]
        public string Location { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}

