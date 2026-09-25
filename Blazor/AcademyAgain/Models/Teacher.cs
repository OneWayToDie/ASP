using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Teacher:Human
	{
		[Key]
		[Column("teacher_id", TypeName = "SMALLINT")]
		public int teacher_id { get; set; }
		public DateOnly work_since { get; set; }

		[DataType(DataType.Currency)]
		[Column(TypeName = "SMALLMONEY")]
		public decimal? rate { get; set; }

	}
}
