using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class CompleteDiscipline
	{
		[Required]
		public int group { get; set; }

		[Required]
		[Column(TypeName = "SMALLINT")]
		public int discipline { get; set; }
	}
}