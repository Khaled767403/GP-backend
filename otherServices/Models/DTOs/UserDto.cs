namespace otherServices.Models.DTOs
{
    public class UserDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public string FileBase64 { get; set; }
    }
}
