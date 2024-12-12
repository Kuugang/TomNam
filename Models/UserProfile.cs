using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TomNam.Models
{
    public abstract class UserProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required string UserId { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public required User User { get; set; }
    }
}