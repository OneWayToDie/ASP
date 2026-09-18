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

		// Возраст — вычисляемое свойство (хранится в БД только дата рождения).
		// Get-only: значение не хранится, а считается на лету при каждом обращении.
		public int Age
		{
			get
			{
				// Сегодняшняя дата как DateOnly (без времени), чтобы сравнивать с birth_date.
				var today = DateOnly.FromDateTime(DateTime.Today);
				// Начальная оценка возраста = разница календарных лет.
				var age = today.Year - birth_date.Year;
				// Если день рождения в этом году ещё не наступил (birth_date сдвинутое на age лет позже сегодня),
				// разница лет завышена на единицу — отнимаем 1.
				if (birth_date.AddYears(age) > today) age--;
				return age;
			}
		}


	}
}
