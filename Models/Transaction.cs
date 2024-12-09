using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TomNam.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public Reservation Reservation { get; set; }

        [Required]
        public double Total { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
    }
}