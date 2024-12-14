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

    public class ReservationResponseDTO(Reservation Reservation)
    {
        public Guid Id { get; set; } = Reservation.Id;
        public Karenderya Karenderya { get; set; } = Reservation.Karenderya;
        public DateTime ReserveDateTime { get; set; } = Reservation.ReserveDateTime;
        public double Total { get; set; } = Reservation.Total;
        public string ModeOfPayment { get; set; } = Reservation.ModeOfPayment;
        public string Status { get; set; } = Reservation.Status;
        [JsonPropertyName("reservedItems")]
        public List<ReservedItem> ReservedItems { get; set; } = (List<ReservedItem>)Reservation.ReservedItems;
    }
}