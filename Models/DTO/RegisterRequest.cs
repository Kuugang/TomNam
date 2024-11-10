using System.ComponentModel.DataAnnotations;

namespace TomNam.Models.DTO
{
    public enum Role
    {
        Customer,
        Owner,
    }

    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(Role), ErrorMessage = "User Role must be either 'User', or 'Owner'")]
        public string UserRole{ get; set; }
    }
}