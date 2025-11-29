namespace WebAPIDotNet.DTOs
{
    public class RegisterResponseDTO
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int FlagWaitingUser { get; set; }
        public string FileBase64 { get; set; }
        public string FileName { get; set; }

    }
}

