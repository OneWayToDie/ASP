using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademyAgain.Models
{
	public class Human
	{
		[Required]
		[StringLength(50, MinimumLength = 2)]
		[RegularExpression("^[A-ZА-Я][a-zа-я]+$")]
		public string last_name {get; set; }
		[Required]
		[StringLength(50, MinimumLength = 2)]
		public string first_name {get; set; }
		public string? middle_name {get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateOnly birth_date  {get; set; }
		[EmailAddress]
		public string? email { get; set; }
		[Phone]
		public string? phone { get; set; }
		[Column("photo", TypeName = "IMAGE")]
		public byte[]? photo { get; set; }

		//			Calculated properties:
		public string FullName
		{
			get => $"{last_name} {first_name} {middle_name}";
		}


	}
}
