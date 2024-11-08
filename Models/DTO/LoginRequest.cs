using System.ComponentModel.DataAnnotations;
namespace TomNam.Models.DTO
{
    public class LoginRequest : IValidatableObject
    {
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Phone))
            {
                yield return new ValidationResult("Please provide email or phone", new[] { "Missing Argument "});
            }
        }
    }
}