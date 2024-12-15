using System.ComponentModel.DataAnnotations;

namespace TomNam.Models.DTO
{
    public class FoodDTO(Food food)
    {
        public Guid? Id { get; set; } = food.Id;
        public Guid? KarenderyaId { get; set; } = food.KarenderyaId;
        public string? FoodName { get; set; } = food.FoodName;
        public string? FoodDescription { get; set; } = food.FoodDescription;
        public double? UnitPrice { get; set; } = food.UnitPrice;
        public string? FoodPhoto { get; set; } = food.FoodPhoto;
        public class CreateFood
        {
            [Required]
            public required string FoodName { get; set; }
            public string? FoodDescription { get; set; }
            [Required]
            public required double UnitPrice { get; set; }
            [Required]
            public required IFormFile FoodPhoto { get; set; }
        }

        public class ReadFood
        {
            public ReadFood() { } // Parameterless constructor for model binding

            public ReadFood(Food food)
            {
                KarenderyaId = food.KarenderyaId;
                FoodName = food.FoodName;
            }

            public Guid? KarenderyaId { get; set; }
            public string? FoodName { get; set; }
        }

        public class UpdateFood
        {
            public Guid? Id { get; set; }
            public string? FoodName { get; set; }
            public string? FoodDescription { get; set; }
            public double? UnitPrice { get; set; }
            public IFormFile? FoodPhoto { get; set; }
        }
    }
}

