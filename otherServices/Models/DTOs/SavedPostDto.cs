using CommentAPI.DTOs;

namespace otherServices.Models.DTOs
{
    public class SavedPostDto
    {
        public long PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Location { get; set; }
        public string RentalStatus { get; set; }
        public long FlagWaitingPost { get; set; }
        public string ImagePath { get; set; }
        public string? FileBase64 { get; set; }
        public DateTime DatePost { get; set; }

        // Landlord
        public long landlordId { get; set; }
        public string landlordUserName { get; set; }

        // Comments
        public List<PostsCommentsDto> Comments { get; set; }
    }
}
