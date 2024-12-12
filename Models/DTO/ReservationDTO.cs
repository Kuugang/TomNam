using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TomNam.Models.DTO
{
    public class ReservationRequestDTO
    {
        public class Create
        {
            [Required]
            public required List<Guid> CartItemIds { get; set; }

            [Required]
            public required DateTime ReserveDateTime { get; set; }

            [Required]
            public required string ModeOfPayment { get; set; }
        }
    }

    public class ReservationResponseDTO
    {
        public Guid Id { get; set; }    
        public required Guid CustomerProfileId { get; set; }
        public required CustomerProfile Customer { get; set; }
        public required Karenderya Karenderya { get; set; }
        public required DateTime ReserveDateTime { get; set; }
        public required double Total { get; set; }
        public required string ModeOfPayment { get; set; }
        [JsonPropertyName("reservedItems")]
        public List<ReservedItem> ReservedItems { get; set; } = new();


    }

    
}
