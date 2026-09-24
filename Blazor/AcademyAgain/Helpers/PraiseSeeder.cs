using AcademyAgain.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademyAgain.Helpers
{
	public static class PraiseSeeder
	{
		public static async Task SeedAsync(AcademyAgainContext context)
		{
			if (await context.PraiseTemplates.AnyAsync())
			{
				return;
			}

			var templates = new (string Text, int Category)[]
			{
				("Хорошо объясняет материал", 1),
				("Объясняет, пока не поймёт вся группа", 1),
				("Зацикливается на важных деталях", 1),
				("Приводит понятные примеры", 1),
				("Умеет заинтересовать предметом", 1),
				("Терпеливо отвечает на вопросы", 1),
				("Занятие прошло по плану", 2),
				("Материал объяснён на базовом уровне", 2),
				("Темп занятия комфортный", 2),
				("Слишком быстрый темп", 3),
				("Мало примеров и практики", 3),
				("Часто отвлекается от темы", 3),
				("Пришлось разбираться самому", 3)
			};

			var order = 1;
			foreach (var t in templates)
			{
				context.PraiseTemplates.Add(new PraiseTemplate
				{
					text = t.Text,
					category = t.Category,
					sort_order = order++,
					is_active = true
				});
			}
			await context.SaveChangesAsync();
		}
	}
}