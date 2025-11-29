namespace otherServices.Models.DTOs
{
    public class ProposalDto
    {
        public long ProposalId { get; set; }
        public long PostId { get; set; }

        public long LandlordId { get; set; }
        public string? LandlordName { get; set; }
        public long TenantId { get; set; }
        public string? TenantName { get; set; }
        public string Phone { get; set; }
        public DateTime StartRentalDate { get; set; }
        public DateTime EndRentalDate { get; set; }
        public string RentalStatus { get; set; }
        public string FileName { get; set; }
        public string FileBase64 { get; set; }
    }
}