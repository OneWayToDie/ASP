using AcademyAgain.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AcademyAgain.Helpers
{
	public static class AuthStore
	{
		private const int Iterations = 600_000;

		public static string HashPassword(string password)
		{
			var salt = RandomNumberGenerator.GetBytes(16);
			var subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, Iterations, 32);
			return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(subkey);
		}

		public static bool VerifyPassword(string password, string stored)
		{
			var parts = stored.Split(':');
			if (parts.Length != 2)
			{
				return false;
			}
			try
			{
				var salt = Convert.FromBase64String(parts[0]);
				var expected = Convert.FromBase64String(parts[1]);
				var actual = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, Iterations, expected.Length);
				return CryptographicOperations.FixedTimeEquals(actual, expected);
			}
			catch (FormatException)
			{
				return false;
			}
		}

		public static async Task EnsureUsersSchemaAsync(AcademyAgainContext context)
		{
			await context.Database.ExecuteSqlRawAsync(
				"IF OBJECT_ID('dbo.Users', 'U') IS NULL CREATE TABLE [dbo].[Users] (" +
				"[user_id] INT IDENTITY(1,1) NOT NULL, " +
				"[username] NVARCHAR(50) NOT NULL, " +
				"[password_hash] NVARCHAR(200) NOT NULL, " +
				"[role_id] TINYINT NOT NULL, " +
				"CONSTRAINT [PK_Users] PRIMARY KEY ([user_id]), " +
				"CONSTRAINT [UQ_Users_username] UNIQUE ([username]))");

			await context.Database.ExecuteSqlRawAsync(
				"IF OBJECT_ID('dbo.Roles', 'U') IS NULL CREATE TABLE [dbo].[Roles] (" +
				"[role_id] TINYINT NOT NULL, " +
				"[role_name] NVARCHAR(20) NOT NULL, " +
				"CONSTRAINT [PK_Roles] PRIMARY KEY ([role_id]))");

			await context.Database.ExecuteSqlRawAsync(
				"IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [role_id] = 1) " +
				"INSERT INTO [dbo].[Roles] ([role_id], [role_name]) VALUES (1, N'admin')");
			await context.Database.ExecuteSqlRawAsync(
				"IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [role_id] = 2) " +
				"INSERT INTO [dbo].[Roles] ([role_id], [role_name]) VALUES (2, N'teacher')");
			await context.Database.ExecuteSqlRawAsync(
				"IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [role_id] = 3) " +
				"INSERT INTO [dbo].[Roles] ([role_id], [role_name]) VALUES (3, N'student')");

			if (!await context.Users.AnyAsync(u => u.username == "admin"))
			{
				context.Users.Add(new User
				{
					username = "admin",
					password_hash = HashPassword("admin123"),
					role_id = 1
				});
				await context.SaveChangesAsync();
			}
		}
	}
}