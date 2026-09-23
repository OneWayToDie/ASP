using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Exam
	{
		[Required]
		public int student { get; set; }

		[Required]
		[Column(TypeName = "SMALLINT")]
		public int discipline { get; set; }

		[Column(TypeName = "DATE")]
		public DateTime? date { get; set; }

		[Column(TypeName = "TINYINT")]
		public int? grade { get; set; }

		[Required]
		[Column(TypeName = "BIT")]
		public bool is_deleted { get; set; }
	}
}