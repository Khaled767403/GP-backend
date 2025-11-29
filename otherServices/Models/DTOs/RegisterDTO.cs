using System.ComponentModel.DataAnnotations;

namespace WebAPIDotNet.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role_name { get; set; }

        public IFormFile? File { get; set; }


    }
}