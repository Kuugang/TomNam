using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TomNam.Models
{
	public class CustomerProfile 
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		public required string UserId { get; set; }

		[Required]
        [ForeignKey("UserId")]
        public required User User { get; set; }

        public int? BehaviorScore { get; set; } 
	}
}