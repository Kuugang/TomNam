using TomNam.Models;
using TomNam.Models.DTO;
using System.Security.Claims;
namespace TomNam.Interfaces{
    public interface ICartItemService
    {
        Task<CartItem?> GetById(Guid Id, string UserId);
        Task<CartItem?> GetByFoodId(Guid FoodId, string UserId);
        Task<CartItem> Create(CustomerProfile CustomerProfile, Food Food, CartRequestItemDTO.Create Request);
        Task<CartItem> Update(CartItem CartItem, CartRequestItemDTO.Update Request);
        Task<List<CartItemReponseDTO>> GetAll(string UserId);
        Task Delete(CartItem CartItem);
    }
}