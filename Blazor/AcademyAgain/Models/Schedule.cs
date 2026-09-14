using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Schedule
	{
		[Key]
		[Required]
		[Column(TypeName = "BIGINT")]
		public long lesson_id { get; set; }

		[Required]
		public int group { get; set; }

		[Required]
		[Column(TypeName = "SMALLINT")]
		public int discipline { get; set; }

		[Required]
		[Column(TypeName = "SMALLINT")]
		public int teacher { get; set; }

		[Column(TypeName = "DATE")]
		public DateTime? date { get; set; }

		[Column(TypeName = "TIME")]
		public TimeSpan? time { get; set; }

		[Required]
		[Column(TypeName = "BIT")]
		public bool spent { get; set; }
	}
}