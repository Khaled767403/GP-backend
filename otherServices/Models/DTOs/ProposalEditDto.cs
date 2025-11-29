using Microsoft.AspNetCore.Mvc;

namespace otherServices.Models.DTOs
{
    public class ProposalEditDto
    {
        [FromForm]
        public string Phone { get; set; }

        [FromForm]
        public DateTime StartRentalDate { get; set; }

        [FromForm]
        public DateTime EndRentalDate { get; set; }

        [FromForm]
        public IFormFile File { get; set; }
    }
}
