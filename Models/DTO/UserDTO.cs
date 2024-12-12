namespace TomNam.Models.DTO
{
	public class UserDTO
	{
		public required string Id { get; set; }
		public required string Email { get; set; }
		public required string Role { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }

		public class CustomerProfileDTO : UserDTO
		{
			public required int BehaviorScore { get; set; }
		}
		
		public class OwnerProfileDTO : UserDTO
		{
			public Guid? KarenderyaId { get; set; }
		}
	}
}
