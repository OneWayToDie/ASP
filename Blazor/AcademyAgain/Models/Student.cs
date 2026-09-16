using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Student:Human
	{
		[Key]
		public int stud_id { get; set; }
		[Required]
		[ForeignKey(nameof(Group))]
		public int group { get; set; }

		//			Navigation propertis:
		public Group Group { get; set; }
	}
}
