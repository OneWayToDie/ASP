using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Teacher
	{
		[Key]
		[Required]
		[Column(TypeName = "SMALLINT")]
		public int teacher_id { get; set; }

		[Column(TypeName = "NVARCHAR(50)")]
		public string? last_name { get; set; }

		[Column(TypeName = "NVARCHAR(50)")]
		public string? first_name { get; set; }

		[Column(TypeName = "NVARCHAR(50)")]
		public string? middle_name { get; set; }

		[Column(TypeName = "DATE")]
		public DateTime? birth_date { get; set; }

		[Column(TypeName = "NVARCHAR(50)")]
		public string? email { get; set; }

		[Column(TypeName = "NCHAR(16)")]
		public string? phone { get; set; }

		[Column(TypeName = "IMAGE")]
		public byte[]? photo { get; set; }

		[Column(TypeName = "DATE")]
		public DateTime? work_since { get; set; }

		[Column(TypeName = "SMALLMONEY")]
		public decimal? rate { get; set; }

		[Required]
		[Column(TypeName = "BIT")]
		public bool is_deleted { get; set; }
	}
}