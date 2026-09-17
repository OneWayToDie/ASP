using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Semester
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int semester_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(50)")]
		public string name { get; set; } = string.Empty;

		[Column(TypeName = "DATE")]
		public DateTime? start_date { get; set; }

		[Column(TypeName = "DATE")]
		public DateTime? end_date { get; set; }

		[Column(TypeName = "BIT")]
		public bool is_even_week_start { get; set; }

		[Column(TypeName = "BIT")]
		public bool is_current { get; set; }
	}
}