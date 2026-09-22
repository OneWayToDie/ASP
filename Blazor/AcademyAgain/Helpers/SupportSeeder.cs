using AcademyAgain.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademyAgain.Helpers
{
	public static class SupportSeeder
	{
		private const string DefaultPassword = "Support2026!";

		public static async Task SeedAsync(AcademyAgainContext context)
		{
			var workers = new (string Login, string Last, string First, string Middle)[]
			{
				("support1", "Соколова", "Анна", "Викторовна"),
				("support2", "Ковалёв", "Дмитрий", "Андреевич"),
				("support3", "Морозова", "Елена", "Сергеевна"),
				("support4", "Игнатьев", "Никита", "Олегович"),
				("support5", "Белова", "Мария", "Павловна")
			};

			var existing = await context.Users.AsNoTracking().Where(u => workers.Select(w => w.Login).Contains(u.username)).Select(u => u.username).ToHashSetAsync();

			foreach (var w in workers)
			{
				if (existing.Contains(w.Login))
				{
					continue;
				}
				context.Users.Add(new User
				{
					username = w.Login,
					password_hash = AuthStore.HashPassword(DefaultPassword),
					role_id = 7,
					status = 1,
					last_name = w.Last,
					first_name = w.First,
					middle_name = w.Middle,
					email = $"{w.Login}@academy.local"
				});
			}
			await context.SaveChangesAsync();
		}
	}
}