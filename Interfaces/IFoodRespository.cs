using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces{
	public interface IFoodRepository
	{
		Task<Food?> GetById(Guid Id);
		Task<Food?> GetByIdWithKarenderya(Guid Id);
		Task Create(Food food);
		Task <bool> UniqueFoodName(Guid KarenderyaId, string FoodName);
		Task<List<Food>> FilterFood(FoodDTO.ReadFood search);
		Task Update(Food food);
		Task Delete(Food food);
	}
}