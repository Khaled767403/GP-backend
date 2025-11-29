using System.ComponentModel.DataAnnotations;

namespace WebAPIDotNet.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Username or email is required")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
