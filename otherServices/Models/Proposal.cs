using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace otherServices.Models
{
    public class Proposal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProposalId { get; set; }

        [Required]
        public long PostId { get; set; }

        [Required]
        public long TenantId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Phone { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime StartRentalDate { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime EndRentalDate { get; set; }

        [Required]
        [MaxLength(255)]
        public string FilePath { get; set; }

        [Required]
        [MaxLength(255)]
        public string RentalStatus { get; set; }

        [ForeignKey("TenantId")]
        [JsonIgnore]
        public virtual User User { get; set; }

        [ForeignKey("PostId")]
        [JsonIgnore]
        public virtual Post Post { get; set; }
    }
}
