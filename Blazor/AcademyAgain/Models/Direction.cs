using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	[Table("Directions")]
	public class Direction
	{
		[Key]
		[Column(TypeName = ("TINYINT"))]
		public int direction_Id { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 2)]
		[Column(TypeName = ("NVARCHAR(50)"))]
		public string direction_Name { get; set; }

		//				Navigation properties:
		public ICollection<Group> Groups { get; set; }
	}
}
