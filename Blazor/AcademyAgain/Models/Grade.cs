using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Grade
	{
		[Required]
		public int student { get; set; }

		[Required]
		[Column(TypeName = "BIGINT")]
		public long lesson { get; set; }

		[Column(TypeName = "TINYINT")]
		public int? grade_1 { get; set; }

		[Column(TypeName = "TINYINT")]
		public int? grade_2 { get; set; }
	}
}