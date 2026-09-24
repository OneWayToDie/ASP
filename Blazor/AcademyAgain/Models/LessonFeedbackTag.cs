using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class LessonFeedbackTag
	{
		[Required]
		public int feedback_id { get; set; }

		[Required]
		public int template_id { get; set; }
	}
}