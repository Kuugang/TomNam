using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TomNam.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }

        [Required]
        public required Guid CustomerProfileId { get; set; }

        [Required]
        [ForeignKey("CustomerProfileId")]
        public required CustomerProfile Customer { get; set; }

        [Required]
        public required Guid FoodId { get; set; }

        [Required]
        [ForeignKey("FoodId")]
        public required Food Food { get; set; }

        [Required]
        public required int Quantity { get; set; }

        public bool IsChecked  { get; set; } = true;
    }
}
