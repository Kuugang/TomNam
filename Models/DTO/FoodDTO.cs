using System.ComponentModel.DataAnnotations;

namespace TomNam.Models.DTO{
    public class FoodDTO{
        public class CreateFood{
            public Guid? Id { get; set; }
            [Required]
            public required string FoodName { get; set; }
            public string? FoodDescription { get; set;}
            [Required]
            public required double UnitPrice { get; set; }
            [Required]
            public required IFormFile FoodPhoto { get; set; }
        }

        public class ReadFood {
            
            public Guid? Id { get; set; }
            public String? KarenderyaId { get; set; }
            public string? FoodName { get; set; }
            public string? FoodDescription { get; set;}
            public double UnitPrice { get; set; }
            public String? FoodPhoto { get; set; }
        }

        public class UpdateFood{
            public Guid? Id { get; set; }
            public string? FoodName { get; set; }
            public string? FoodDescription { get; set;}
            public double? UnitPrice { get; set; }
            public IFormFile? FoodPhoto { get; set; }
        }

        // final String Id;
        // final String karenderyaId;
        // final String foodName;
        // final String foodDescription;
        // final double unitPrice;
        // final String? foodPhoto;
    }
}

