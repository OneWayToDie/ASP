using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Project
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int project_id { get; set; }

		[Required]
		public int user_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(100)")]
		public string title { get; set; } = string.Empty;

		[Column(TypeName = "NVARCHAR(1000)")]
		public string? description { get; set; }

		[Column(TypeName = "NVARCHAR(200)")]
		public string? tags { get; set; }

		[Column(TypeName = "NVARCHAR(300)")]
		public string? url { get; set; }

		[Required]
		[Column(TypeName = "TINYINT")]
		public int status { get; set; }

		[Required]
		[Column(TypeName = "BIT")]
		public bool pinned { get; set; }

		[Column(TypeName = "IMAGE")]
		public byte[]? cover { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime? created_at { get; set; }
	}
}