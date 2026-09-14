using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Salary
	{
		[Key]
		[Required]
		[Column(TypeName = "BIGINT")]
		public long payment_id { get; set; }

		[Required]
		[Column(TypeName = "SMALLINT")]
		public int teacher { get; set; }

		[Required]
		[Column(TypeName = "SMALLMONEY")]
		public decimal accrued { get; set; }

		[Required]
		[Column(TypeName = "BIT")]
		public bool received { get; set; }
	}
}