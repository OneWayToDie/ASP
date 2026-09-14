using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class DaysOFF
	{
		[Key]
		[Required]
		[Column(TypeName = "DATE")]
		public DateTime date { get; set; }

		[Required]
		[Column(TypeName = "TINYINT")]
		public int holiday { get; set; }
	}
}