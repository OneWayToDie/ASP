using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Holiday
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Required]
		[Column(TypeName = "TINYINT")]
		public int holiday_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(150)")]
		public string holiday_name { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "TINYINT")]
		public int duration { get; set; }

		[Column(TypeName = "TINYINT")]
		public int? month { get; set; }

		[Column(TypeName = "TINYINT")]
		public int? day { get; set; }
	}
}