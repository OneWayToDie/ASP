using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class News
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int news_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(200)")]
		public string title { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "NVARCHAR(MAX)")]
		public string body { get; set; } = string.Empty;

		[Column(TypeName = "DATETIME")]
		public DateTime? published_at { get; set; }

		[Column(TypeName = "BIT")]
		public bool is_published { get; set; }

		[Column(TypeName = "INT")]
		public int? author_id { get; set; }
	}
}