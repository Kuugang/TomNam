using Microsoft.EntityFrameworkCore;
using TomNam.Data;
using TomNam.Interfaces;
using TomNam.Models;

namespace TomNam.Services
{
    public class CartItemRepository : ICartItemRepository
    {
        DataContext _context;
        public CartItemRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<CartItem?> GetById(Guid Id, string UserId)
        {
            return await _context.CartItem.FirstOrDefaultAsync(x => x.Id == Id && x.Customer.User.Id == UserId);
        }

        public async Task<List<CartItem>> GetByIds(List<Guid> CartItemIds, string UserId)
        {
            return await _context.CartItem
                .Where(ci => CartItemIds.Contains(ci.Id) && ci.Customer.User.Id == UserId)
                .Include(ci => ci.Food)
                .ThenInclude(f => f.Karenderya)
                .ToListAsync();
        }

        public async Task<CartItem?> GetByFoodId(Guid FoodId, string UserId)
        {
            return await _context.CartItem.FirstOrDefaultAsync(x => x.FoodId == FoodId && x.Customer.User.Id == UserId);
        }

        public async Task Create(CartItem CartItem)
        {
            _context.CartItem.Add(CartItem);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CartItem CartItem)
        {
            _context.CartItem.Update(CartItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CartItem>> GetAll(string UserId)
        {
            return await _context.CartItem
                .Include(x => x.Customer)
                .Include(x => x.Food)
                .Where(x => x.Customer.User.Id == UserId)
                .ToListAsync();
        }

        public async Task Delete(CartItem CartItem)
        {
            _context.CartItem.Remove(CartItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItems(List<CartItem> cartItems)
        {
            _context.CartItem.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}