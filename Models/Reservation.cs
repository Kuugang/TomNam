using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TomNam.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }

        [Required]
        public required Guid CustomerProfileId { get; set; }

        [Required]
        [ForeignKey("CustomerProfileId")]
        public required CustomerProfile Customer { get; set; }

        [Required]
        public required Guid KarenderyaId { get; set; }

        [Required]
        [ForeignKey("KarenderyaId")]
        public required Karenderya  Karenderya { get; set; }

        [Required]
        public required DateTime  ReserveDateTime { get; set; }
        
        [Required]
        public required double Total { get; set; }

        [Required]
        public required string ModeOfPayment { get; set; }

        [JsonIgnore]
        public ICollection<ReservedItem> ReservedItems { get; set; } = new List<ReservedItem>();

        
    }
}
