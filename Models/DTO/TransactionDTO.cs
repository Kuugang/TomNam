using System.ComponentModel.DataAnnotations;

namespace TomNam.Models.DTO
{
    public class TransactionRequestDTO
    {
        public class Create
        {
            [Required]
            public Guid ReservationId { get; set; }
        }

    }

    public class TransactionResponseDTO
    {
        public string TransactionId { get; set; }
        public Guid ReservationId { get; set; }
        public Guid ReserveItemId { get; set; }
        public int ReserveItemQuantity { get; set; }
        public decimal Total { get; set; }
        public DateTime DateTime { get; set; }

    }

   
}
