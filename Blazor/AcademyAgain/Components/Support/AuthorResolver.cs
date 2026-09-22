using AcademyAgain.Helpers;
using AcademyAgain.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademyAgain.Components.Support
{
	public sealed record AuthorInfo(int UserId, string Name, string? PhotoSrc, string? DataSrc, string Initials, bool IsSupport);

	public static class AuthorResolver
	{
		public static string FullName(User u)
		{
			var parts = new[] { u.last_name, u.first_name, u.middle_name }.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
			if (parts.Length > 0)
			{
				return string.Join(" ", parts);
			}
			return string.IsNullOrWhiteSpace(u.username) ? $"Пользователь #{u.user_id}" : u.username;
		}

		public static string Initials(User u)
		{
			var chars = new[] { u.first_name, u.last_name }
				.Where(p => !string.IsNullOrWhiteSpace(p))
				.Select(p => char.ToUpperInvariant(p[0]))
				.ToArray();
			if (chars.Length >= 2)
			{
				return $"{chars[0]}{chars[1]}";
			}
			if (chars.Length == 1)
			{
				return chars[0].ToString();
			}
			return !string.IsNullOrWhiteSpace(u.username) ? char.ToUpperInvariant(u.username[0]).ToString() : "?";
		}

		public static AuthorInfo Build(User u, bool isSupport, byte[]? linkedPhoto = null)
		{
			var photo = u.photo is { Length: > 0 } ? u.photo : linkedPhoto;
			var name = FullName(u);
			return new AuthorInfo(u.user_id, name, Photo.AvatarUrl(u.user_id, photo), Photo.DataSrc(photo), Initials(u), isSupport);
		}

		public static async Task<Dictionary<int, byte[]>> LoadLinkedPhotosAsync(AcademyAgainContext db, IReadOnlyCollection<User> users)
		{
			var photos = new Dictionary<int, byte[]>();
			var missing = users.Where(u => u.photo is not { Length: > 0 } && u.linked_id is int).ToList();
			if (missing.Count == 0)
			{
				return photos;
			}

			var studentIds = missing.Where(u => u.role_id == 3).Select(u => u.linked_id!.Value).ToArray();
			if (studentIds.Length > 0)
			{
				var rows = await db.Students.AsNoTracking()
					.Where(s => studentIds.Contains(s.stud_id) && s.photo != null)
					.Select(s => new { s.stud_id, s.photo })
					.ToListAsync();
				foreach (var r in rows)
				{
					if (r.photo is { Length: > 0 })
					{
						photos[r.stud_id] = r.photo;
					}
				}
			}

			var teacherIds = missing.Where(u => u.role_id == 2).Select(u => u.linked_id!.Value).ToArray();
			if (teacherIds.Length > 0)
			{
				var rows = await db.Teachers.AsNoTracking()
					.Where(t => teacherIds.Contains(t.teacher_id) && t.photo != null)
					.Select(t => new { t.teacher_id, t.photo })
					.ToListAsync();
				foreach (var r in rows)
				{
					if (r.photo is { Length: > 0 })
					{
						photos[r.teacher_id] = r.photo;
					}
				}
			}

			return photos;
		}
	}
}