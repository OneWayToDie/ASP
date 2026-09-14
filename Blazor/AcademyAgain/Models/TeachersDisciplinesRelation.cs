using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class TeachersDisciplinesRelation
	{
		[Required]
		[Column(TypeName = "SMALLINT")]
		public int teacher { get; set; }

		[Required]
		[Column(TypeName = "SMALLINT")]
		public int discipline { get; set; }
	}
}