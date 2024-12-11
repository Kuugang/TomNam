using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TomNam.Models
{
	public class OwnerProfile
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		public required string UserId { get; set; }

		[Required]
        [ForeignKey("UserId")]
        public required User User { get; set; }

        [JsonIgnore] // To avoid circular references
        public Karenderya? Karenderya { get; set; }
	}
}