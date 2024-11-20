using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TomNam.Models
{
	public class User : IdentityUser
	{
		[Required]
		public required string FirstName { get; set; }
		[Required]
		public required string LastName { get; set; }
	}
}