using AcademyAgain.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AcademyAgain.Helpers
{
	public static class AuthStore
	{
		private const int Iterations = 600_000;

		public static ClaimsPrincipal CreatePrincipal(string username, string roleName)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.Name, username),
				new Claim(ClaimTypes.Role, roleName)
			};
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			return new ClaimsPrincipal(identity);
		}

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

		public static async Task SeedAsync(AcademyAgainContext context)
		{
			var roles = new (int Id, string Name)[]
			{
				(1, "admin"),
				(2, "teacher"),
				(3, "student"),
				(4, "moderator"),
				(5, "candidate"),
				(6, "teacher_candidate"),
				(7, "support")
			};

			foreach (var (id, name) in roles)
			{
				if (!await context.Roles.AnyAsync(r => r.role_id == id))
				{
					context.Roles.Add(new Role { role_id = id, role_name = name });
				}
			}
			await context.SaveChangesAsync();

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
		}
	}
}