using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Discipline
	{
		[Key]
		[Column(TypeName = "SMALLINT")]
		public int discipline_id { get; set; }

		[Required]
		public string discipline_name { get; set; }

		[Required]
		[Column(TypeName = "TINYINT")]
		public int number_of_lessons { get; set; }
	}
}
