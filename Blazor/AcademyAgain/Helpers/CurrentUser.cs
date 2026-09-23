using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademyAgain.Helpers
{
	/// <summary>
	/// Разрешает user_id текущего пользователя (из claim Username -> БД) и кэширует на скоуп.
	/// </summary>
	public class CurrentUser(
		IDbContextFactory<AcademyAgainContext> dbFactory,
		AuthenticationStateProvider authStateProvider)
	{
		private int? _userId;
		private bool _resolved;

		public async Task<int?> GetIdAsync()
		{
			if (_resolved)
			{
				return _userId;
			}
			_resolved = true;

			var authState = await authStateProvider.GetAuthenticationStateAsync();
			var username = authState.User.FindFirst(ClaimTypes.Name)?.Value;
			if (string.IsNullOrWhiteSpace(username))
			{
				return _userId;
			}

			await using var db = await dbFactory.CreateDbContextAsync();
			_userId = await db.Users.AsNoTracking()
				.Where(u => u.username == username)
				.Select(u => (int?)u.user_id)
				.FirstOrDefaultAsync();
			return _userId;
		}
	}
}