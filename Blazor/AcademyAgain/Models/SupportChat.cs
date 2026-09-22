using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class SupportChat
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int chat_id { get; set; }

		[Required]
		[Column(TypeName = "INT")]
		public int user_id { get; set; }

		[Required]
		[Column(TypeName = "TINYINT")]
		public int status { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime? created_at { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime? updated_at { get; set; }
	}
}