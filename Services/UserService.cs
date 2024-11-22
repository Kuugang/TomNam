using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using TomNam.Models;
using TomNam.Data;
using TomNam.Interfaces;

namespace TomNam.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        // Constructor injection to get the DbContext
        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Id == userId);
        }
    }
}
