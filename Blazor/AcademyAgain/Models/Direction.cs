using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Direction
	{
		[Key]
		[Required]
		[Column(TypeName = "TINYINT")]
		public int direction_id { get; set; }

		[Column(TypeName = "NVARCHAR(50)")]
		public string? direction_name { get; set; }
	}
}