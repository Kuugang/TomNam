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
        public Guid TransactionId { get; set; }
        public Guid ReservationId { get; set; }
        public double Total { get; set; }
        public DateTime DateTime { get; set; }

    }
}
