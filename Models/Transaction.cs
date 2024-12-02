using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TomNam.Models.DTO;

namespace TomNam.Models
{
    public class Transaction
    {
        [Key]
        public string TransactionId { get; set; }

        [Required]
        public Guid ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public Reservation Reservation { get; set; }

        [Required]
        public Guid ReserveItemId { get; set; }

        [ForeignKey("ReserveItemId")]
        public ReservedItem ReservedItem { get; set; }

        [Required]
        public int ReserveItemQuantity { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
    }
}