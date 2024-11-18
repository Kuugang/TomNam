using System.ComponentModel.DataAnnotations;

namespace TomNam.Models.DTO
{
	public class KarenderyaDTO
	{


		[Required]
		public required string KarenderyaId { get; set; } // uuid of karenderya
		[Required]
		public required string Name { get; set; }
		[Required]
		public required string LocationStreet { get; set; }
		[Required]
		public required string LocationBarangay { get; set; }
		[Required]
		public required string LocationCity { get; set; }
		[Required]
		public required string LocationProvince { get; set; }
		public string? Description { get; set; }
		public string? LogoPhoto { get; set; }
		public string? CoverPhoto { get; set; }

	  public class Create
	  {
		[Required]
		public required string Name { get; set; }
		[Required]
		public required string LocationStreet { get; set; }
		[Required]
		public required string LocationBarangay { get; set; }
		[Required]
		public required string LocationCity { get; set; }
		[Required]
		public required string LocationProvince { get; set; }
		public string? Description { get; set; }
	  }
	}
	
	public class ProofOfBusinessDTO 
	{
		public class Create 
		{
			[Required]
			public required string OwnerValidID1 { get; set; }
			[Required]
			public required string OwnerValidID2 { get; set; }
			[Required]
			public required string BusinessPermit { get; set; }
			[Required]
			public required string BIRPermit { get; set; }
		}
	}
}