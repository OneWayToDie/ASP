using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Student
	{
		[Key]
		[Required]
		public int stud_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(50)")]
		public string last_name { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "NVARCHAR(50)")]
		public string first_name { get; set; } = string.Empty;

		[Column(TypeName = "NVARCHAR(50)")]
		public string? middle_name { get; set; }

		[Required]
		[Column(TypeName = "DATE")]
		public DateTime birth_date { get; set; }

		[Column(TypeName = "NVARCHAR(50)")]
		public string? email { get; set; }

		[Column(TypeName = "NCHAR(16)")]
		public string? phone { get; set; }

		[Column(TypeName = "IMAGE")]
		public byte[]? photo { get; set; }

		public int? group { get; set; }
	}
}