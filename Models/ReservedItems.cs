using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TomNam.Models
{
    public class ReservedItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public Reservation Reservation { get; set; }

        [Required]
        public Guid FoodId { get; set; }

        [ForeignKey("FoodId")]
        public Food Food { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}