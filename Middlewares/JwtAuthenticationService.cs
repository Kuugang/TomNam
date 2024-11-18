using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;


using TomNam.Models;
using TomNam.Interfaces;

namespace TomNam.Middlewares
{
    public class JwtAuthenticationService
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey;
        private readonly IConfiguration _configuration;

        public JwtAuthenticationService(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
            _secretKey = configuration["JWT:Secret"] ?? throw new ArgumentNullException("JWT:Secret configuration is missing.");
        }

        // Optional constructor for dependency injection
        public JwtAuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = configuration["JWT:Secret"] ?? throw new ArgumentNullException("JWT:Secret configuration is missing.");
        }

        public string GenerateToken(string userId, string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),  // Username as 'Name' claim (optional, for better readability)
                new Claim(ClaimTypes.NameIdentifier, userId), // Use ClaimTypes.NameIdentifier for user ID
                new Claim(ClaimTypes.Role, role), // Role as a ClaimTypes.Role claim
                new Claim(ClaimTypes.Sid, userId), // This can also be used for user ID (alternative)
                new Claim(ClaimTypes.Upn, username), // UserPrincipalName for the username (if needed)

                // Optional: iat (Issued At) can be represented as a Unix timestamp
                new Claim(ClaimTypes.DateOfBirth, DateTime.UtcNow.ToString("yyyy-MM-dd")), // Example custom claim for DOB (optional)
            };

            // Set token expiration (optional)
            var expiration = DateTime.UtcNow.AddHours(1); // Token expires in 1 hour

            var secret = _configuration["JWT:Secret"] ?? throw new ArgumentNullException("JWT:Secret configuration is missing.");
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenDescriptor); // Create the token as a string

            return token;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var principal = ValidateToken(token);
                if (principal != null)
                {
                    context.User = principal;  // Sets the User on HttpContext.
                }
            }

            await _next(context);  // Continue to the next middleware.
        }


        private ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);  // Secret key from your configuration.

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch (Exception e)
            {
                Console.WriteLine("Token validation failed: " + e.Message);
                return null;  // Token validation failed
            }
        }

        public static async Task<User> GetUserFromTokenAsync(ClaimsPrincipal principal, IUserService userService)
        {
            if (principal == null) return null;
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;

            var userId = userIdClaim.Value;
            return await userService.GetUserByIdAsync(userId);  // UserService handles fetching user from DB
        }
    }
}