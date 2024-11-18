using System.ComponentModel.DataAnnotations;

namespace TomNam.Models.DTO
{
    public class LoginResponse
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Id { get; set; }
        public required string Role { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required bool EmailConfirmed { get; set; }
        public required string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}