using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Attendance
	{
		[Required]
		public int student { get; set; }

		[Required]
		[Column(TypeName = "BIGINT")]
		public long lesson { get; set; }

		[Required]
		[Column(TypeName = "BIT")]
		public bool present { get; set; }
	}
}