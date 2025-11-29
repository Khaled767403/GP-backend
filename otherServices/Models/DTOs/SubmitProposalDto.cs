using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace otherServices.Models.DTOs
{
    public class SubmitProposalDto
    {
        //public long PostId { get; set; }
        public long TenantId { get; set; }
        public string Phone { get; set; }
        public DateTime StartRentalDate { get; set; }
        public DateTime EndRentalDate { get; set; }

        public IFormFile File { get; set; }

    }
}
