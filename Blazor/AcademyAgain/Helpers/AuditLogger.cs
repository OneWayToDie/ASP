using AcademyAgain.Models;

namespace AcademyAgain.Helpers
{
	/// <summary>
	/// Пишет строки в журнал аудита. Сервис scoped: при создании в рамках запроса/сцена
	/// подтягивает user_id отправителя действия.
	/// </summary>
	public class AuditLogger(CurrentUser currentUser)
	{
		public Task LogAsync(
			AcademyAgainContext db,
			string action,
			string entity,
			string? entityId,
			string? oldValue = null,
			string? newValue = null)
		{
			db.AuditLogs.Add(new AuditLog
			{
				user_id = null,
				action = action,
				entity = entity,
				entity_id = entityId,
				old_value = oldValue,
				new_value = newValue,
				created_at = DateTime.Now
			});
			return Task.CompletedTask;
		}

		public Task LogAsync(
			AcademyAgainContext db,
			int? userId,
			string action,
			string entity,
			string? entityId,
			string? oldValue = null,
			string? newValue = null)
		{
			db.AuditLogs.Add(new AuditLog
			{
				user_id = userId,
				action = action,
				entity = entity,
				entity_id = entityId,
				old_value = oldValue,
				new_value = newValue,
				created_at = DateTime.Now
			});
			return Task.CompletedTask;
		}

		public async Task<int?> ResolveUserIdAsync() => await currentUser.GetIdAsync();

		/// <summary>
		/// Добавляет запись аудита и сразу сохраняет (для действий, выполненных
		/// вне ChangeTracker — например ExecuteUpdateAsync).
		/// </summary>
		public async Task LogAndSaveAsync(
			AcademyAgainContext db,
			string action,
			string entity,
			string? entityId,
			string? oldValue = null,
			string? newValue = null,
			CancellationToken ct = default)
		{
			db.AuditLogs.Add(new AuditLog
			{
				user_id = await ResolveUserIdAsync(),
				action = action,
				entity = entity,
				entity_id = entityId,
				old_value = oldValue,
				new_value = newValue,
				created_at = DateTime.Now
			});
			await db.SaveChangesAsync(ct);
		}
	}
}