using System.ComponentModel.DataAnnotations;

namespace TomNam.Models.DTO{
    public class FoodDTO{
        public Guid? Id { get; set; }
        [Required]
        public required string FoodName { get; set; }
        public string? FoodDescription { get; set;}
        [Required]
        public required double UnitPrice { get; set; }
		[MaxLength(2048)]
		public string? FoodPhoto { get; set; }
    }
}

