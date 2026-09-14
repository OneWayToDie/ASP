using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class RequiredDiscipline
	{
		[Required]
		[Column(TypeName = "SMALLINT")]
		public int discipline { get; set; }

		[Required]
		[Column(TypeName = "SMALLINT")]
		public int required_discipline { get; set; }
	}
}