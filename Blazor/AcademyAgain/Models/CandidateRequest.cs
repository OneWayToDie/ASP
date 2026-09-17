using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class CandidateRequest
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }

		[Required]
		[Column(TypeName = "INT")]
		public int candidate_user_id { get; set; }

		[Required]
		[Column(TypeName = "INT")]
		public int teacher_id { get; set; }

		[Required]
		[Column(TypeName = "TINYINT")]
		public int status { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime? created_at { get; set; }
	}
}