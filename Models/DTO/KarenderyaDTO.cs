using System.ComponentModel.DataAnnotations;

namespace TomNam.Models.DTO 
{
	public class KarenderyaDTO{
	

		public class EditRequest{

		}
		
		public class Create{
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
		}
	}
}