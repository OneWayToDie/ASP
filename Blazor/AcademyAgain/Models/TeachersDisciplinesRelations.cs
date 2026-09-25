using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class TeachersDisciplinesRelations
	{
		[Column("teacher", TypeName = "SMALLINT")]
		[ForeignKey(nameof(Teacher))]
		public int teacher { get; set; }
		
		[Column("discipline", TypeName = "SMALLINT")]
		[ForeignKey(nameof(Discipline))]
		public int discipline { get; set; }

		//NAvigation properties
		public Teacher Teacher { get; set; }
		public Discipline Discipline { get; set; }
	}
}
