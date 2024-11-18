using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using TomNam.Models.DTO;

namespace TomNam.Helpers
{
    public static class TokenHelper
    {
        /// <summary>
        /// Extracts the bearer token from the Authorization header.
        /// </summary>
        /// <param name="httpContext">The current HTTP context.</param>
        /// <returns>The token string or null if not present.</returns>
        public static string? GetBearerToken(HttpContext httpContext)
        {
            var bearerToken = httpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(bearerToken) || !bearerToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return bearerToken.Substring("Bearer ".Length).Trim();
        }

        /// <summary>
        /// Extracts the User Response DTO from a JWT token's "User" claim.
        /// </summary>
        /// <param name="token">The JWT token string.</param>
        /// <returns>The user ID as a string, or null if not found.</returns>
        public static LoginResponse? ExtractUserDTO(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            // Validate the token format
            if (!handler.CanReadToken(token))
            {
                return null;
            }

            // Read the token
            var jwtToken = handler.ReadJwtToken(token);

            // Get the "User" claim
            var userClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "User")?.Value;
            if (string.IsNullOrEmpty(userClaim))
            {
                return null;
            }

            try
            {
                var user = JsonSerializer.Deserialize<LoginResponse>(userClaim);
                return user;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Extracts the user's role from a JWT token.
        /// </summary>
        /// <param name="token">The JWT token string.</param>
        /// <returns>The role as a string, or null if not found.</returns>
        public static string? ExtractUserRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            // Validate the token format
            if (!handler.CanReadToken(token))
            {
                return null;
            }

            // Read the token
            var jwtToken = handler.ReadJwtToken(token);

            // Get the role claim
            return jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
        }
    }
}