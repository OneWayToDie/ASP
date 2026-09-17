using AcademyAgain.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AcademyAgain.Helpers
{
	public enum VerificationPurpose
	{
		Registration = 1,
		PasswordReset = 2
	}

	public enum VerifyStatus
	{
		Ok,
		NotFound,
		Expired,
		InvalidCode,
		TooManyAttempts
	}

	public sealed record VerificationCodePayload(
		string Username,
		string PasswordHash,
		int RoleId,
		string LastName,
		string FirstName,
		string? MiddleName,
		string? Phone);

	public sealed class VerifyResult
	{
		public VerifyStatus Status { get; init; }
		public EmailVerificationCode? Code { get; init; }
	}

	public class EmailVerificationService(
		IDbContextFactory<AcademyAgainContext> dbFactory,
		IEmailSender emailSender)
	{
		private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(15);
		private const int MaxAttempts = 5;

		public static string GenerateCode() => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

		public async Task IssueAsync(
			string email,
			VerificationPurpose purpose,
			VerificationCodePayload? pending = null,
			int? userId = null)
		{
			email = Normalize(email);
			await using var db = await dbFactory.CreateDbContextAsync();

			var history = await db.EmailVerificationCodes
				.Where(c => c.email == email && c.purpose == (int)purpose)
				.OrderByDescending(c => c.id)
				.ToListAsync();

			if (pending is null && purpose == VerificationPurpose.Registration)
			{
				var source = history.FirstOrDefault(c => c.pending_username != null);
				if (source is not null)
				{
					pending = new VerificationCodePayload(
						source.pending_username!,
						source.pending_password_hash!,
						source.pending_role_id ?? 5,
						source.pending_last_name ?? string.Empty,
						source.pending_first_name ?? string.Empty,
						source.pending_middle_name,
						source.pending_phone);
				}
			}

			foreach (var old in history.Where(c => !c.consumed))
			{
				old.consumed = true;
			}

			var code = GenerateCode();
			db.EmailVerificationCodes.Add(new EmailVerificationCode
			{
				email = email,
				purpose = (int)purpose,
				code_hash = AuthStore.HashPassword(code),
				expires_at = DateTime.Now.Add(Lifetime),
				created_at = DateTime.Now,
				consumed = false,
				attempts = 0,
				user_id = userId,
				pending_username = pending?.Username,
				pending_password_hash = pending?.PasswordHash,
				pending_role_id = pending?.RoleId,
				pending_last_name = pending?.LastName,
				pending_first_name = pending?.FirstName,
				pending_middle_name = pending?.MiddleName,
				pending_phone = pending?.Phone
			});
			await db.SaveChangesAsync();

			var subject = purpose == VerificationPurpose.Registration
				? "Подтверждение регистрации — Academy"
				: "Восстановление пароля — Academy";
			var body =
				$"<p>Ваш код: <strong style=\"font-size:20px\">{code}</strong></p>" +
				$"<p>Код действует {Lifetime.TotalMinutes:0} минут. Если вы не запрашивали код, просто проигнорируйте письмо.</p>";

			await emailSender.SendAsync(email, subject, body);
		}

		public async Task<VerifyResult> VerifyAsync(string email, VerificationPurpose purpose, string code)
		{
			email = Normalize(email);
			code = (code ?? string.Empty).Trim();

			await using var db = await dbFactory.CreateDbContextAsync();
			var entity = await db.EmailVerificationCodes
				.Where(c => c.email == email && c.purpose == (int)purpose && !c.consumed)
				.OrderByDescending(c => c.id)
				.FirstOrDefaultAsync();

			if (entity is null)
			{
				return new VerifyResult { Status = VerifyStatus.NotFound };
			}
			if (entity.expires_at < DateTime.Now)
			{
				entity.consumed = true;
				await db.SaveChangesAsync();
				return new VerifyResult { Status = VerifyStatus.Expired };
			}
			if (entity.attempts >= MaxAttempts)
			{
				entity.consumed = true;
				await db.SaveChangesAsync();
				return new VerifyResult { Status = VerifyStatus.TooManyAttempts };
			}
			if (!AuthStore.VerifyPassword(code, entity.code_hash))
			{
				entity.attempts++;
				if (entity.attempts >= MaxAttempts)
				{
					entity.consumed = true;
				}
				await db.SaveChangesAsync();
				return new VerifyResult { Status = VerifyStatus.InvalidCode };
			}

			entity.consumed = true;
			await db.SaveChangesAsync();
			return new VerifyResult { Status = VerifyStatus.Ok, Code = entity };
		}

		private static string Normalize(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();
	}
}
