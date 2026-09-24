using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class TeacherReview
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int review_id { get; set; }

		[Required]
		public int teacher_id { get; set; }

		[Required]
		public int student_id { get; set; }

		[Required]
		[Range(1, 5)]
		[Column(TypeName = "TINYINT")]
		public int rating { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(1000)")]
		public string text { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "DATETIME")]
		public DateTime created_at { get; set; }

		[Required]
		[Column(TypeName = "BIT")]
		public bool is_deleted { get; set; }
	}
}