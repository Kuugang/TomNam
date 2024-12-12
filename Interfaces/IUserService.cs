using TomNam.Models;
using TomNam.Models.DTO;
using System.Security.Claims;
namespace TomNam.Interfaces{
	public interface IUserService
	{
		Task<User?> GetUserByIdAsync(string userId);
		Task<User?> GetUserByEmail(string email);
		Task<SuccessResponseDTO> Register(RegisterRequest RegisterRequest);
		Task<SuccessResponseDTO> Login(LoginRequest LoginRequest);
		string GenerateToken(User user, string role);
		Task<string> GetRole(User user);
		Task<User?> GetUserFromToken(ClaimsPrincipal principal);
		Task <OwnerProfile?> GetOwnerProfile(string userId);
		Task <CustomerProfile?> GetCustomerProfile(string userId);
		string? GetUserIdFromToken(ClaimsPrincipal principal);
		Task UpdateOwnerProfile(OwnerProfile ownerProfile, Karenderya karenderya);
	}
}