using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TomNam.Models
{
	public class ProofOfBusiness
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		public required string KarenderyaId { get; set; }

		[Required]
		[ForeignKey("KarenderyaId")]
		public required Karenderya Karenderya { get; set; }

		[Required]
		[MaxLength(2048)]
		public required string OwnerValidID1 { get; set; }

		[Required]
		[MaxLength(2048)]
		public required string OwnerValidID2 { get; set; }
		
		[Required]
		[MaxLength(2048)]
		public required string BusinessPermit { get; set; }

		[Required]
		[MaxLength(2048)]
		public required string BIRPermit { get; set; }
	}
}