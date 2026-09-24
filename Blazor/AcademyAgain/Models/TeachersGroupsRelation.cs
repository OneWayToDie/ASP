using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class TeachersGroupsRelation
	{
		[Required]
		[Column(TypeName = "SMALLINT")]
		public int teacher { get; set; }

		[Required]
		public int group { get; set; }
	}
}