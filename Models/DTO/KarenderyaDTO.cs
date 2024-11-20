using System.ComponentModel.DataAnnotations;

namespace TomNam.Models.DTO
{
	public class KarenderyaRequestDTO 
	{
		public class Read 
		{
			public Guid? KarenderyaId { get; set; } // For getting 1 karenderya
			public string? Name { get; set; } // For getting karenderyas based on Name
			public string? LocationStreet { get; set; } // For getting karenderyas based on LocationStreet
			public string? LocationBarangay { get; set; } // For getting karenderyas based on LocationBarangay
			public string? LocationCity { get; set; } // For getting karenderyas based on LocationCity
			public string? LocationProvince { get; set; } // For getting karenderyas based on LocationProvince
		}
		public class Update 
		{
			public string? Name { get; set; }
			public string? LocationStreet { get; set; }
			public string? LocationBarangay { get; set; }
			public string? LocationCity { get; set; }
			public string? LocationProvince { get; set; }
			public DateOnly? DateFounded { get; set; }
			public string? Description { get; set; }
			public string? LogoPhoto { get; set; }
			public string? CoverPhoto { get; set; }
		}

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
			[Required]
			public required DateOnly DateFounded { get; set; }
			public string? Description { get; set; }
		}
		
		public class ProofOfBusinessCreateDTO 
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
		
		public class ProofOfBusinessUpdateDTO 
		{
			public string? OwnerValidID1 { get; set; }
			public string? OwnerValidID2 { get; set; }
			public string? BusinessPermit { get; set; }
			public string? BIRPermit { get; set; }
		}
	}
	
	public class KarenderyaResponseDTO 
	{
		public Guid? Id { get; set; } // uuid of karenderya
		public string? Name { get; set; }
		public string? LocationStreet { get; set; }
		public string? LocationBarangay { get; set; }
		public string? LocationCity { get; set; }
		public string? LocationProvince { get; set; }
		public DateOnly? DateFounded { get; set; }
		public string? Description { get; set; }
		public string? LogoPhoto { get; set; }
		public string? CoverPhoto { get; set; }
	}
}