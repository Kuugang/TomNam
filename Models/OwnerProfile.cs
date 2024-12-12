using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TomNam.Models
{
    public class OwnerProfile : UserProfile
    {
        public Guid? KarenderyaId { get; set; }

        [ForeignKey("KarenderyaId")]
        [JsonIgnore]
        public Karenderya? Karenderya { get; set; }
    }
}