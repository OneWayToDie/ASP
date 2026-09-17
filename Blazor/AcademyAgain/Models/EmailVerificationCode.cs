using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class EmailVerificationCode
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(100)")]
		public string email { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "TINYINT")]
		public int purpose { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(200)")]
		public string code_hash { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "DATETIME")]
		public DateTime expires_at { get; set; }

		[Required]
		[Column(TypeName = "DATETIME")]
		public DateTime created_at { get; set; }

		[Required]
		[Column(TypeName = "BIT")]
		public bool consumed { get; set; }

		[Required]
		[Column(TypeName = "TINYINT")]
		public int attempts { get; set; }

		[Column(TypeName = "INT")]
		public int? user_id { get; set; }

		[Column(TypeName = "NVARCHAR(50)")]
		public string? pending_username { get; set; }

		[Column(TypeName = "NVARCHAR(200)")]
		public string? pending_password_hash { get; set; }

		[Column(TypeName = "TINYINT")]
		public int? pending_role_id { get; set; }

		[Column(TypeName = "NVARCHAR(50)")]
		public string? pending_last_name { get; set; }

		[Column(TypeName = "NVARCHAR(50)")]
		public string? pending_first_name { get; set; }

		[Column(TypeName = "NVARCHAR(50)")]
		public string? pending_middle_name { get; set; }

		[Column(TypeName = "NVARCHAR(100)")]
		public string? pending_phone { get; set; }
	}
}
