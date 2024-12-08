using TomNam.Models;
using TomNam.Models.DTO;
using System.Security.Claims;
namespace TomNam.Interfaces{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByEmail(string email);
        Task<User> Create(RegisterRequest RegisterRequest);
        Task<string> Login(LoginRequest LoginRequest);
        Task<string> GenerateToken(User user, string role);
        Task<string> GetRole(User user);
        Task<User> GetUserFromToken(ClaimsPrincipal principal);
        var userHasKarenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.UserId == user.Id);
    }
}