using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TomNam.Models
{
	public class CustomerProfile : UserProfile 
	{
        public int? BehaviorScore { get; set; } 
	}
}