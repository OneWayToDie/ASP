using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class SupportMessage
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int msg_id { get; set; }

		[Required]
		[Column(TypeName = "INT")]
		public int chat_id { get; set; }

		[Required]
		[Column(TypeName = "INT")]
		public int author_user_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(MAX)")]
		public string body { get; set; } = string.Empty;

		[Column(TypeName = "DATETIME")]
		public DateTime? created_at { get; set; }
	}
}