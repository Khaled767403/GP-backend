using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebAPIDotNet.DTOs
{
    public class UpdatePostDTO
    {

        [FromForm]
        [StringLength(255)]
        public string Title { get; set; }
        [FromForm]
        public string Description { get; set; }
        [FromForm]
        public double? Price { get; set; }
        [FromForm]
        [StringLength(255)]
        public string Location { get; set; }
        [FromForm]
        [StringLength(50)]
        public string RentalStatus { get; set; }

        [FromForm]
        public IFormFile File { get; set; }
    }
}