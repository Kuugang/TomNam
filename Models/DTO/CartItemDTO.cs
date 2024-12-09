using System.ComponentModel.DataAnnotations;

namespace TomNam.Models.DTO
{
    public class CartRequestItemDTO
    {
        public class Create
        {
            [Required]
            public required Guid FoodId { get; set; }

            [Required]
            public required int Quantity { get; set; }
            
        }

        public class Update
        {
            public int Quantity { get; set; }

            public bool IsChecked { get; set; }
        }
    }

    public class CartItemReponseDTO
    {
        public Guid Id { get; set; }

        public CustomerProfile Customer { get; set; }

        public Food Food { get; set; }

        public int Quantity { get; set; }

        public bool IsChecked { get; set; }
    }
}
