using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TomNam.Models{
    public class Food{
        public Guid Id { get; set; }
		[Required]
		public required Guid KarenderyaId { get; set; }
		[Required]
		[ForeignKey("KarenderyaId")]
		public required Karenderya Karenderya { get; set; }
        [Required]
        public required string FoodName { get; set; }
        [Required]
        public required string FoodDescription { get; set;}
        [Required]
        public required double UnitPrice { get; set; }
		[MaxLength(2048)]
		public string? FoodPhoto { get; set; }
    }
}