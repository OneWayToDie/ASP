using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class DisciplinesDirectionsRelation
	{
		[Required]
		[Column(TypeName = "TINYINT")]
		public int direction { get; set; }

		[Required]
		[Column(TypeName = "SMALLINT")]
		public int discipline { get; set; }
	}
}