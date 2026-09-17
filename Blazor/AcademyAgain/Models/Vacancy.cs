using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Vacancy
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int vacancy_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(200)")]
		public string title { get; set; } = string.Empty;

		[Column(TypeName = "NVARCHAR(MAX)")]
		public string? description { get; set; }

		[Column(TypeName = "INT")]
		public int? discipline_id { get; set; }

		[Column(TypeName = "BIT")]
		public bool is_open { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime? created_at { get; set; }
	}
}