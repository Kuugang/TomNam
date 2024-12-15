using Microsoft.EntityFrameworkCore;
using TomNam.Data;
using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces
{
	public class FoodRepository : IFoodRepository
	{
		private readonly DataContext _context;

		public FoodRepository(DataContext context)
		{
			_context = context;
		}
		public async Task<Food?> GetById(Guid Id)
		{
			return await _context.Food.FindAsync(Id);
		}
		
		public async Task<Food?> GetByIdWithKarenderya(Guid Id)
		{
			return await _context.Food.Include(x => x.Karenderya).FirstOrDefaultAsync(f => f.Id == Id);
		}
		
		public async Task Create(Food food)
		{
			await _context.Food.AddAsync(food);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> UniqueFoodName(Guid KarenderyaId, string FoodName)
		{
			var foodCount = await _context.Food.CountAsync(f => f.FoodName == FoodName && f.KarenderyaId == KarenderyaId);
			return foodCount == 0;
		}

		public async Task<List<Food>> FilterFood(FoodDTO.ReadFood filter)
		{
			var query = _context.Food.AsQueryable();
			if (filter.FoodName != null)
			{
				query = query.Where(f => f.FoodName.ToLower().Contains(filter.FoodName.ToLower()));
			}

			if (filter.KarenderyaId != null)
			{
				query = query.Where(f => f.KarenderyaId == filter.KarenderyaId);
			}
			return await query.ToListAsync();
		}

		public async Task Update(Food food){
			_context.Food.Update(food);
			await _context.SaveChangesAsync();
		}
		public async Task Delete(Food food){
			_context.Food.Remove(food);
			await _context.SaveChangesAsync();
		}
	}
}