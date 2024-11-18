using TomNam.Models;
namespace TomNam.Interfaces{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(string userId);
    }
}