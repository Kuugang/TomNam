using System.Security.Claims;
using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces{
    public interface ICartItemService
    {
        Task<CartItem?> GetById(Guid Id, string UserId);
        Task<List<CartItem>> GetByIds(List<Guid> CartItemIds, string UserId);
        Task<CartItem?> GetByFoodId(Guid FoodId, string UserId);
        Task<CartItem> Create(CartRequestItemDTO.Create Request, ClaimsPrincipal User);
        Task<CartItem> Update(Guid CartItemId, CartRequestItemDTO.Update Request, ClaimsPrincipal User);
        Task<List<CartItem>> GetAll(ClaimsPrincipal User);
        Task Delete(Guid CartItemId, ClaimsPrincipal User);
        Task DeleteItems(List<CartItem> CartItems);
    }
}