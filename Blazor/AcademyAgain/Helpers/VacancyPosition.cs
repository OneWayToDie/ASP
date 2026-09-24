namespace AcademyAgain.Helpers
{
	public static class VacancyPosition
	{
		public const int DefaultRoleId = 2;

		public static readonly int[] FunnelRoleIds = { 2, 7, 8, 9, 10, 11 };

		public static string LabelFor(int? roleId) => roleId switch
		{
			2 => "Преподаватель",
			7 => "Сотрудник техподдержки",
			8 => "Бухгалтер",
			9 => "Маркетолог по набору",
			10 => "Куратор учебного процесса",
			11 => "Инженер платформы",
			_ => "Преподаватель"
		};
	}
}