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
				"[status] TINYINT NOT NULL CONSTRAINT [DF_Users_status] DEFAULT 0, " +
				"[linked_id] INT NULL, " +
				"[photo] IMAGE NULL, " +
				"[banner] IMAGE NULL, " +
				"[bio] NVARCHAR(300) NULL, " +
				"[tagline] NVARCHAR(80) NULL, " +
				"CONSTRAINT [PK_Users] PRIMARY KEY ([user_id]), " +
				"CONSTRAINT [UQ_Users_username] UNIQUE ([username]))");

			var hasStatus = await context.Database.SqlQueryRaw<int>(
				"SELECT COUNT(*) AS [Value] FROM [sys].[columns] " +
				"WHERE [object_id] = OBJECT_ID('dbo.Users') AND [name] = 'status'").FirstOrDefaultAsync();
			if (hasStatus == 0)
			{
				await context.Database.ExecuteSqlRawAsync(
					"ALTER TABLE [dbo].[Users] ADD [status] TINYINT NOT NULL " +
					"CONSTRAINT [DF_Users_status] DEFAULT 0");
				await context.Database.ExecuteSqlRawAsync(
					"UPDATE [dbo].[Users] SET [status] = 1");
			}

			var hasLinkedId = await context.Database.SqlQueryRaw<int>(
				"SELECT COUNT(*) AS [Value] FROM [sys].[columns] " +
				"WHERE [object_id] = OBJECT_ID('dbo.Users') AND [name] = 'linked_id'").FirstOrDefaultAsync();
			if (hasLinkedId == 0)
			{
				await context.Database.ExecuteSqlRawAsync(
					"ALTER TABLE [dbo].[Users] ADD [linked_id] INT NULL");
			}

			var hasPhoto = await context.Database.SqlQueryRaw<int>(
				"SELECT COUNT(*) AS [Value] FROM [sys].[columns] " +
				"WHERE [object_id] = OBJECT_ID('dbo.Users') AND [name] = 'photo'").FirstOrDefaultAsync();
			if (hasPhoto == 0)
			{
				await context.Database.ExecuteSqlRawAsync(
					"ALTER TABLE [dbo].[Users] ADD [photo] IMAGE NULL");
			}

			var hasBanner = await context.Database.SqlQueryRaw<int>(
				"SELECT COUNT(*) AS [Value] FROM [sys].[columns] " +
				"WHERE [object_id] = OBJECT_ID('dbo.Users') AND [name] = 'banner'").FirstOrDefaultAsync();
			if (hasBanner == 0)
			{
				await context.Database.ExecuteSqlRawAsync(
					"ALTER TABLE [dbo].[Users] ADD [banner] IMAGE NULL");
			}

			var hasBio = await context.Database.SqlQueryRaw<int>(
				"SELECT COUNT(*) AS [Value] FROM [sys].[columns] " +
				"WHERE [object_id] = OBJECT_ID('dbo.Users') AND [name] = 'bio'").FirstOrDefaultAsync();
			if (hasBio == 0)
			{
				await context.Database.ExecuteSqlRawAsync(
					"ALTER TABLE [dbo].[Users] ADD [bio] NVARCHAR(300) NULL");
			}

			var hasTagline = await context.Database.SqlQueryRaw<int>(
				"SELECT COUNT(*) AS [Value] FROM [sys].[columns] " +
				"WHERE [object_id] = OBJECT_ID('dbo.Users') AND [name] = 'tagline'").FirstOrDefaultAsync();
			if (hasTagline == 0)
			{
				await context.Database.ExecuteSqlRawAsync(
					"ALTER TABLE [dbo].[Users] ADD [tagline] NVARCHAR(80) NULL");
			}

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
			await context.Database.ExecuteSqlRawAsync(
				"IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [role_id] = 4) " +
				"INSERT INTO [dbo].[Roles] ([role_id], [role_name]) VALUES (4, N'moderator')");
			await context.Database.ExecuteSqlRawAsync(
				"IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [role_id] = 5) " +
				"INSERT INTO [dbo].[Roles] ([role_id], [role_name]) VALUES (5, N'candidate')");

			if (!await context.Users.AnyAsync(u => u.username == "admin"))
			{
				context.Users.Add(new User
				{
					username = "admin",
					password_hash = HashPassword("admin123"),
					role_id = 1,
					status = 1
				});
				await context.SaveChangesAsync();
			}

			await context.Database.ExecuteSqlRawAsync(
				"IF OBJECT_ID('dbo.Projects', 'U') IS NULL CREATE TABLE [dbo].[Projects] (" +
				"[project_id] INT IDENTITY(1,1) NOT NULL, " +
				"[user_id] INT NOT NULL, " +
				"[title] NVARCHAR(100) NOT NULL, " +
				"[description] NVARCHAR(1000) NULL, " +
				"[tags] NVARCHAR(200) NULL, " +
				"[url] NVARCHAR(300) NULL, " +
				"[status] TINYINT NOT NULL, " +
				"[pinned] BIT NOT NULL CONSTRAINT [DF_Projects_pinned] DEFAULT 0, " +
				"[cover] IMAGE NULL, " +
				"[created_at] DATETIME NULL, " +
				"CONSTRAINT [PK_Projects] PRIMARY KEY ([project_id]))");

			await context.Database.ExecuteSqlRawAsync(
				"IF OBJECT_ID('dbo.CandidateRequests', 'U') IS NULL CREATE TABLE [dbo].[CandidateRequests] (" +
				"[id] INT IDENTITY(1,1) NOT NULL, " +
				"[candidate_user_id] INT NOT NULL, " +
				"[teacher_id] INT NOT NULL, " +
				"[status] TINYINT NOT NULL CONSTRAINT [DF_CandidateRequests_status] DEFAULT 0, " +
				"[created_at] DATETIME NULL CONSTRAINT [DF_CandidateRequests_created_at] DEFAULT GETDATE(), " +
				"CONSTRAINT [PK_CandidateRequests] PRIMARY KEY ([id]))");
		}
	}
}