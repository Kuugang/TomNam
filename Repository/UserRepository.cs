using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using TomNam.Models;
using TomNam.Data;
using TomNam.Interfaces;

namespace TomNam.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        // Constructor injection to get the DbContext
        public UserRepository(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Id == userId);
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Email == email);
        }
        public async Task<User> Create(User user, string password, string role){
            //CREATE USER 
            await _userManager.CreateAsync(user, password);

            //CREATE USER ROLE
            await _userManager.AddToRoleAsync(user, role);

            //CREATE USER PROFILE
            switch (role)
            {
                case "Customer":
                    CustomerProfile CustomerProfile = new CustomerProfile
                    {
                        UserId = user.Id,
                        User = user,
                        BehaviorScore = 30
                    };
		            await _context.CustomerProfile.AddAsync(CustomerProfile);
                    break;
                case "Owner":
                    OwnerProfile OwnerProfile = new OwnerProfile
                    {
                        UserId = user.Id,
                        User = user
                    };
		            await _context.OwnerProfile.AddAsync(OwnerProfile);
                    break;
            }

		    await _context.SaveChangesAsync();
            return user;
        }
    }
}
