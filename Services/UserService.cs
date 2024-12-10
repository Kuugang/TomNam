using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Interfaces;
using TomNam.Middlewares;


namespace TomNam.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtAuthenticationService _jwtAuthenticationService;
        private readonly UserManager<User> _userManager;
        // Constructor injection to get the DbContext
        public UserService(IUserRepository userRepository, JwtAuthenticationService jwtAuthenticationService, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _jwtAuthenticationService = jwtAuthenticationService;
            _userManager = userManager;
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }
        public async Task<User> Create(RegisterRequest RegisterRequest)
        {
            var user = new User
            {
                Email = RegisterRequest.Email,
                UserName = RegisterRequest.Email,
                FirstName = RegisterRequest.FirstName,
                LastName = RegisterRequest.LastName,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            return await _userRepository.Create(user, RegisterRequest.Password, RegisterRequest.UserRole);
        }
        public async Task<string?> Login(LoginRequest LoginRequest)
        {
            var user = await GetUserByEmail(LoginRequest.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, LoginRequest.Password)){
                return null;
            }
            return GenerateToken(user, (await _userManager.GetRolesAsync(user))[0]);
        }
        public string GenerateToken(User user, string role){
            return _jwtAuthenticationService.GenerateToken(user.Id, user.UserName!, role);
        }

        public async Task<string> GetRole(User user)
        {
            string role = (await _userManager.GetRolesAsync(user))[0];
            return role;
        }
        public async Task<User?> GetUserFromToken(ClaimsPrincipal principal){
            if (principal == null) return null;
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;

            var userId = userIdClaim.Value;
            return await GetUserByIdAsync(userId);  
        }

        public async Task <OwnerProfile?> GetOwnerProfile(string userId){
            return await _userRepository.GetOwnerProfile(userId);
        }

        public async Task <CustomerProfile?> GetCustomerProfile(string userId){
            return await _userRepository.GetCustomerProfile(userId);
        }

        public string? GetUserIdFromToken(ClaimsPrincipal principal){
            if (principal == null) return null;
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;

            var userId = userIdClaim.Value;
            return userId;
        }
    }
}