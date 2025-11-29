namespace otherServices.Models.DTOs
{
    public class PostDTo
    {
        public long PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Location { get; set; }
        public string RentalStatus { get; set; }
        public DateTime DatePost { get; set; }
        public long FlagWaitingPost { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string ImagePath { get; set; }
        public string FileBase64 { get; set; }





    }
}
