using System.ComponentModel.DataAnnotations;
using System.Text.Json;
namespace TomNam.Models.DTO
{
    public class AccessTokenResponse
    {
        /// <summary>
        /// The type of the token (e.g., "Bearer").
        /// </summary>
        public string? TokenType { get; set; } // Nullable since it can be null

        /// <summary>
        /// The access token used for authentication.
        /// </summary>
        public string? AccessToken { get; set; } // Nullable since it can be null

        /// <summary>
        /// The duration in seconds until the access token expires.
        /// </summary>
        public long ExpiresIn { get; set; } // Not nullable since it's required

        /// <summary>
        /// A token that can be used to refresh the access token.
        /// </summary>
        public string? RefreshToken { get; set; } // Nullable since it can be null
    }

}
