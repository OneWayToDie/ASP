using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Role
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Required]
		[Column(TypeName = "TINYINT")]
		public int role_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(20)")]
		public string role_name { get; set; } = string.Empty;
	}
}