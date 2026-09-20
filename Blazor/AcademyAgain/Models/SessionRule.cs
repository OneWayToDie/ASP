using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class SessionRule
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int session_rule_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(100)")]
		public string name { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "TINYINT")]
		public int min_attendance_pct { get; set; }

		[Required]
		[Column(TypeName = "DECIMAL(3,2)")]
		public decimal min_average_grade { get; set; }

		[Required]
		[Column(TypeName = "TINYINT")]
		public int required_makeups { get; set; }

		[Required]
		[Column(TypeName = "BIT")]
		public bool is_active { get; set; }
	}
}