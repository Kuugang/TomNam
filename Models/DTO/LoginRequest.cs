using System.ComponentModel.DataAnnotations;
namespace TomNam.Models.DTO
{
    public class LoginRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}