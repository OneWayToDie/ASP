using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Group
	{
		[Key]
		[Required]
		public int group_id { get; set; }

		[Column(TypeName = "NCHAR(10)")]
		public string? group_name { get; set; }

		[Column(TypeName = "TINYINT")]
		public int? direction { get; set; }

		[Column(TypeName = "TINYINT")]
		public int? weekdays { get; set; }

		[Column(TypeName = "TIME")]
		public TimeSpan? start_time { get; set; }

		[Column(TypeName = "DATE")]
		public DateTime? start_date { get; set; }
	}
}