using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class AdmissionRequest
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(150)")]
		public string full_name { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "NVARCHAR(100)")]
		public string contact { get; set; } = string.Empty;

		[Column(TypeName = "INT")]
		public int? direction_id { get; set; }

		[Column(TypeName = "TINYINT")]
		public int status { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime? created_at { get; set; }

		[Required]
		[Column(TypeName = "BIT")]
		public bool is_deleted { get; set; }
	}
}