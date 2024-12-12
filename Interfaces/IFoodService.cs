using System.Security.Claims;
using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces{
    public interface IFoodService
    {
        Task<Food> GetById(Guid Id);
        Task<Food> Create(Guid KarenderyaId, FoodDTO.CreateFood request, ClaimsPrincipal User);
        Task<bool> UniqueFoodName(Guid KarenderyaId, string FoodName);
        Task<List<Food>> FilterFood(FoodDTO.ReadFood filter);
        Task<Food> Update(Guid FoodId, FoodDTO.UpdateFood request, ClaimsPrincipal User);
        Task Delete(Guid FoodId, ClaimsPrincipal User);
    }
}