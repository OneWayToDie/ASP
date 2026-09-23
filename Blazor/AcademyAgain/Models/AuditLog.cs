using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class AuditLog
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long audit_id { get; set; }

		[Column(TypeName = "INT")]
		public int? user_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(50)")]
		public string action { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "NVARCHAR(100)")]
		public string entity { get; set; } = string.Empty;

		[Column(TypeName = "NVARCHAR(100)")]
		public string? entity_id { get; set; }

		[Column(TypeName = "NVARCHAR(MAX)")]
		public string? old_value { get; set; }

		[Column(TypeName = "NVARCHAR(MAX)")]
		public string? new_value { get; set; }

		[Required]
		[Column(TypeName = "DATETIME")]
		public DateTime created_at { get; set; }
	}
}