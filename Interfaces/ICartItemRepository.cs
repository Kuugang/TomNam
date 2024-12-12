using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces{
    public interface ICartItemRepository
    {
        Task<CartItem?> GetById(Guid Id, string UserId);
        Task<List<CartItem>> GetByIds(List<Guid> CartItemIds, string UserId);
        Task<CartItem?> GetByFoodId(Guid FoodId, string UserId);
        Task<List<CartItem>> GetAll(string UserId);
        Task Create(CartItem CartItem);
        Task Update(CartItem CartItem);
        Task Delete(CartItem CartItem);
        Task DeleteItems(List<CartItem> CartItems);
    }
}