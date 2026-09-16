using System.ComponentModel.DataAnnotations;

namespace AcademyAgain.Models
{
	public class Student:Human
	{
		[Key]
		public int stud_id { get; set; }
		[Required]
		public int group { get; set; }
	}
}
