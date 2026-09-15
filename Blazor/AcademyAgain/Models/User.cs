using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int user_id { get; set; }

		[Required]
		[Column(TypeName = "NVARCHAR(50)")]
		public string username { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "NVARCHAR(200)")]
		public string password_hash { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "TINYINT")]
		public int role_id { get; set; }
	}
}