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
            public int? Quantity { get; set; }

            public bool? IsChecked { get; set; }
        }
    }

    public class CartItemReponseDTO(CartItem CartItem)
    {
        public Guid Id { get; set; } = CartItem.Id;

        public CustomerProfile Customer { get; set; } = CartItem.Customer;

        public Food Food { get; set; } = CartItem.Food;

        public int Quantity { get; set; } = CartItem.Quantity;

        public bool IsChecked { get; set; } = CartItem.IsChecked;
    }
}
