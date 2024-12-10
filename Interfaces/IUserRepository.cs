using TomNam.Models;
namespace TomNam.Interfaces{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByEmail(string email);
        Task<User> Create(User user, string password, string role);
        Task<OwnerProfile?> GetOwnerProfile(string userId);
        Task<CustomerProfile?> GetCustomerProfile(string userId);
    }
}