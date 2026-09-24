using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class LessonFeedback
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int feedback_id { get; set; }

		[Required]
		[Column(TypeName = "BIGINT")]
		public long lesson_id { get; set; }

		[Required]
		public int student_id { get; set; }

		[Required]
		[Range(1, 5)]
		[Column(TypeName = "TINYINT")]
		public int rating { get; set; }

		[Required]
		[Column(TypeName = "DATETIME")]
		public DateTime created_at { get; set; }
	}
}