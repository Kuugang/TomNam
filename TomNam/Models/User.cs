using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TomNam.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}