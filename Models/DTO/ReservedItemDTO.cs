using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TomNam.Models.DTO;

namespace TomNam.Models
{
    public class ReservedItemDTO(ReservedItem reservedItem)
    {
        
        public Guid Id { get; set; } = reservedItem.Id;

        public Guid ReservationId { get; set; } = reservedItem.ReservationId;

        public Guid FoodId { get; set; } = reservedItem.FoodId;

        public FoodDTO Food { get; set; } = new FoodDTO(reservedItem.Food);

        public int Quantity { get; set; } = reservedItem.Quantity;
    }
}