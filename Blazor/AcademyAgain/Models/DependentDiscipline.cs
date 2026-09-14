using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class DependentDiscipline
	{
		[Required]
		[Column(TypeName = "SMALLINT")]
		public int discipline { get; set; }

		[Required]
		[Column(TypeName = "SMALLINT")]
		public int dependent_discipline { get; set; }
	}
}