using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Interfaces;
using TomNam.Middlewares;
using TomNam.Exceptions;


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
        public async Task<SuccessResponseDTO> Register(RegisterRequest RegisterRequest)
        {
            var userByEmail = await GetUserByEmail(RegisterRequest.Email);

            if (userByEmail != null)
            {
                throw new ApplicationExceptionBase(
                    $"User with email {RegisterRequest.Email} already exists.",
                    "User registration failed.",
                    StatusCodes.Status409Conflict
                );
            }

            var user = new User
            {
                Email = RegisterRequest.Email,
                UserName = RegisterRequest.Email,
                FirstName = RegisterRequest.FirstName,
                LastName = RegisterRequest.LastName,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            await _userRepository.Create(user, RegisterRequest.Password, RegisterRequest.UserRole);
            var role = await GetRole(user);

            var token = GenerateToken(user, role);

            return new SuccessResponseDTO
            {
                Message = "User created successfully",
                Data = new JWTDTO
                {
                    Token = token
                }
            };
        }

        public async Task<SuccessResponseDTO> Login(LoginRequest LoginRequest)
        {
            var user = await GetUserByEmail(LoginRequest.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, LoginRequest.Password))
            {
                throw new ApplicationExceptionBase(
                    "Invalid credentials.",
                    "Login failed.",
                    StatusCodes.Status401Unauthorized
                );
            }

            var token = GenerateToken(user, (await _userManager.GetRolesAsync(user))[0]);
            return new SuccessResponseDTO
            {
                Message = "Login successful",
                Data = new JWTDTO { Token = token }
            };
        }
        public string GenerateToken(User user, string role)
        {
            return _jwtAuthenticationService.GenerateToken(user.Id, user.UserName!, role);
        }

        public async Task<string> GetRole(User user)
        {
            string role = (await _userManager.GetRolesAsync(user))[0];
            return role;
        }
        public async Task<User?> GetUserFromToken(ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;

            var userId = userIdClaim.Value;
            return await GetUserByIdAsync(userId);
        }

        public async Task<OwnerProfile?> GetOwnerProfile(string userId)
        {
            return await _userRepository.GetOwnerProfile(userId);
        }

        public async Task<CustomerProfile?> GetCustomerProfile(string userId)
        {
            return await _userRepository.GetCustomerProfile(userId);
        }

        public async Task<CustomerProfile?> GetCustomerProfileById(string CustomerProfileId)
        {
            return await _userRepository.GetCustomerProfileById(CustomerProfileId);
        }

        public async Task<UserDTO> GetUserProfile(ClaimsPrincipal User)
        {
            var user = await GetUserFromToken(User);
            var roles = await _userManager.GetRolesAsync(user!);
            var role = roles[0];

            if (role == "Customer")
            {
                var customerProfile = await GetCustomerProfile(user!.Id);
                return new UserDTO.CustomerProfileDTO
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Role = role,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BehaviorScore = customerProfile?.BehaviorScore ?? 0,
                };
            }
            else
            {
                var ownerProfile = await GetOwnerProfile(user!.Id);
                return new UserDTO.OwnerProfileDTO
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Role = role,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    KarenderyaId = ownerProfile?.Karenderya?.Id,
                };
            }
        }

        public string? GetUserIdFromToken(ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;

            var userId = userIdClaim.Value;
            return userId;
        }

        public Task UpdateOwnerProfile(OwnerProfile ownerProfile, Karenderya karenderya)
        {
            ownerProfile.Karenderya = karenderya;
            return _userRepository.UpdateOwnerProfile(ownerProfile);
        }

        public async Task UpdateCustomerProfile(CustomerProfile CustomerProfile)
        {
            await _userRepository.UpdateCustomerProfile(CustomerProfile);
        }

    }
}