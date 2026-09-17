using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int user_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(50)")]
		public string username { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "NVARCHAR(200)")]
		public string password_hash { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "TINYINT")]
		public int role_id { get; set; }

		[Required]
		[Column(TypeName = "TINYINT")]
		public int status { get; set; }

		[Column(TypeName = "INT")]
		public int? linked_id { get; set; }

		[Column(TypeName = "IMAGE")]
		public byte[]? photo { get; set; }

		[Column(TypeName = "IMAGE")]
		public byte[]? banner { get; set; }

		[Column(TypeName = "NVARCHAR(300)")]
		public string? bio { get; set; }

		[Column(TypeName = "NVARCHAR(80)")]
		public string? tagline { get; set; }
	}
}