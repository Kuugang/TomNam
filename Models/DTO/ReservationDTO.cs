using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TomNam.Models.DTO;

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
        public KarenderyaResponseDTO Karenderya { get; set; }
        public DateTime ReserveDateTime { get; set; }
        public double Total { get; set; }
        public string ModeOfPayment { get; set; }
        public string Status { get; set; }

        [JsonPropertyName("reservedItems")]
        public List<ReservedItemDTO> ReservedItems { get; set; }

        public ReservationResponseDTO(Reservation reservation)
        {
            Id = reservation.Id;
            Karenderya = new KarenderyaResponseDTO(reservation.Karenderya);
            ReserveDateTime = reservation.ReserveDateTime;
            Total = reservation.Total;
            ModeOfPayment = reservation.ModeOfPayment;
            Status = reservation.Status;
            ReservedItems = reservation.ReservedItems
                .Select(ri => new ReservedItemDTO(ri)) // Map each ReservedItem to ReservedItemDTO
                .ToList();
        }
    }

}