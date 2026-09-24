using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class PraiseTemplate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int template_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(200)")]
		public string text { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "TINYINT")]
		public int category { get; set; }

		[Column(TypeName = "INT")]
		public int? sort_order { get; set; }

		[Required]
		[Column(TypeName = "BIT")]
		public bool is_active { get; set; }
	}
}