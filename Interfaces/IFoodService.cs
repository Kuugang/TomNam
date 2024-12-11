using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces{
    public interface IFoodService
    {
        Task<Food?> GetById(Guid Id);
        Task<Food> Create(Karenderya Karenderya, FoodDTO.CreateFood request);
        Task<bool> UniqueFoodName(Guid KarenderyaId, string FoodName);
        Task<List<FoodDTO.ReadFood>> FilterFood(FoodDTO.ReadFood filter);
        Task<Food> Update(Food food, FoodDTO.UpdateFood request);
        Task Delete(Food food);
    }
}