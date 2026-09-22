using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class SupportBan
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ban_id { get; set; }

		[Required]
		[Column(TypeName = "INT")]
		public int chat_id { get; set; }

		[Required]
		[Column(TypeName = "INT")]
		public int user_id { get; set; }

		[Column(TypeName = "INT")]
		public int? moderator_user_id { get; set; }

		[Column(TypeName = "NVARCHAR(300)")]
		public string? reason { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime? created_at { get; set; }
	}
}