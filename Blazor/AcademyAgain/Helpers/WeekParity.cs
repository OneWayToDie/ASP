namespace AcademyAgain.Helpers
{
	/// <summary>
	/// Чётность учебных недель: отсчёт от start_date семестра
	/// с учётом флага is_even_week_start и учебных дней группы (Group.weekdays). 
	/// Битовая маска: 1=Пн, 2=Вт, 4=Ср, 8=Чт, 16=Пт, 32=Сб, 64=Вс.
	/// </summary>
	public static class WeekParity
	{
		public static bool IsTrainingDay(DateTime date, int? weekdaysMask)
		{
			if (weekdaysMask is null)
			{
				return true;
			}
			int bit = 1 << (((int)date.DayOfWeek + 6) % 7);
			return (weekdaysMask.Value & bit) != 0;
		}

		/// <summary>Порядковый номер недели с начала семестра (1-based). Возвращает 0 до старта.</summary>
		public static int WeekIndex(DateTime date, DateTime semesterStart)
		{
			var start = semesterStart.Date;
			var d = date.Date;
			if (d < start)
			{
				return 0;
			}
			return ((d - start).Days / 7) + 1;
		}

		/// <summary>
		/// Чётная ли неделя. Если is_even_week_start == true, первая неделя семестра
		/// считается чётной, иначе — нечётной.
		/// </summary>
		public static bool IsEvenWeek(DateTime weekMarker, DateTime semesterStart, bool evenWeekStartsSemester)
		{
			int idx = WeekIndex(weekMarker, semesterStart);
			if (idx <= 0)
			{
				return false;
			}
			return (idx % 2 == 1) == evenWeekStartsSemester;
		}

		public static string ParityLabel(bool isEven) => isEven ? "чётная" : "нечётная";
	}
}