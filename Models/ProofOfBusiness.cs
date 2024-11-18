using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TomNam.Models
{
    public class ProofOfBusiness
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required string UserId { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public required User User { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LocationStreet { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LocationBarangay { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LocationCity { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LocationProvince { get; set; }

        [Required]
        public required DateOnly DateFounded { get; set; }

        [Required]
        [MaxLength(500)]
        public required string Description { get; set; }

        [MaxLength(2048)]
        public string? LogoPhoto { get; set; }

        [MaxLength(2048)]
        public string? CoverPhoto { get; set; }

        public double? Rating { get; set; }
    }
}